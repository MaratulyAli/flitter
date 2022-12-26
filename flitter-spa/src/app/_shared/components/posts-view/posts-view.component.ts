import { Component, Input, OnInit } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { KeycloakService } from 'keycloak-angular';
import { finalize } from 'rxjs';
import { IPostCreateOrEditDialogData } from '../../interfaces/dialog-data.interface';
import { ILikeCreate } from '../../models/like.model';
import { IOption, IOptionVoteCreate } from '../../models/poll.model';
import { IPost } from '../../models/post.model';
import { IUser } from '../../models/user.model';
import { PostService } from '../../services/post.service';
import { UserService } from '../../services/user.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { PostCreateOrEditDialogComponent } from '../post-create-or-edit-dialog/post-create-or-edit-dialog.component';

@Component({
  selector: 'app-posts-view',
  templateUrl: './posts-view.component.html',
  styleUrls: ['./posts-view.component.scss']
})
export class PostsViewComponent implements OnInit {
  @Input() posts!: IPost[];
  optionsDisabled = false;
  me!: IUser;
  isAuthenticated = false;

  constructor(
    private _dialog: MatDialog,
    private _postService: PostService,
    private _userService: UserService,
    private _keycloakService: KeycloakService
  ) { }

  async ngOnInit() {
    this.isAuthenticated = await this._keycloakService.isLoggedIn();
    if (this.isAuthenticated) {
      this._userService.getMe().subscribe({
        next: response => {
          if (response.body) {
            this.me = response.body;
          }
        },
        error: e => console.error(e)
      });
    }
  }

  edit(postId: number) {
    const dialogData: IPostCreateOrEditDialogData = { postId };

    this._dialog.open(PostCreateOrEditDialogComponent, {
      width: '578px',
      data: dialogData
    }).afterClosed().subscribe((response?: IPost) => {
      if (response) {
        const post = this.posts.find(p => p.id === postId);

        if (post) {
          post.text = response.text;

          if (response?.poll) {
            post.poll = response.poll;
          } else {
            post.poll = null;
          }
        }
      }
    });
  }

  remove(postId: number) {
    this._dialog.open(ConfirmDialogComponent, {
      width: '378px',
    }).afterClosed().subscribe((response?: boolean) => {
      if (response) {
        this._postService.delete(postId).subscribe({
          next: () => {
            const postIndex = this.posts.findIndex(p => p.id === postId);

            if (postIndex > -1) {
              this.posts.splice(postIndex, 1);
            }
          },
          error: e => console.error(e)
        });
      }
    });
  }

  like(postId: number, button: MatButton) {
    button.disabled = true;

    const body: ILikeCreate = { postId };

    this._postService.like(body)
      .pipe(finalize(() => button.disabled = false))
      .subscribe({
        next: () => {
          const post = this.posts.find(p => p.id === postId);

          if (post) {
            if (post.liked) { post.likesCount--; }
            else { post.likesCount++; }

            post.liked = !post.liked;
          }
        },
        error: e => console.error(e)
      });
  }

  vote(postId: number, pollId: number, optionId: number) {
    this.optionsDisabled = true;

    const body: IOptionVoteCreate = { optionId, pollId, postId };

    this._postService.vote(body)
      .pipe(finalize(() => this.optionsDisabled = false))
      .subscribe({
        next: response => {
          const options = response.body;
          if (options) {
            const post = this.posts.find(p => p.id === postId);

            if (post && post.poll) {
              post.poll.options = options;
            }
          }
        },
        error: e => console.error(e)
      });
  }

  isVoted(post: IPost) {
    return post.poll?.options.some(x => x.voted);
  }

  calculatePercentage(votesCount: number, options: IOption[]) {
    return votesCount / options.map(o => o.votesCount).reduce((a, b) => a + b, 0) * 100;
  }
}
