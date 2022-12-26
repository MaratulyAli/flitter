import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { KeycloakService } from 'keycloak-angular';
import { finalize } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PostCreateOrEditDialogComponent } from '../_shared/components/post-create-or-edit-dialog/post-create-or-edit-dialog.component';
import { IPost } from '../_shared/models/post.model';
import { IUser } from '../_shared/models/user.model';
import { PostService } from '../_shared/services/post.service';
import { UserService } from '../_shared/services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  env = environment;
  isLoading = false;
  posts: IPost[] = [];
  isAuthenticated = false;
  me!: IUser;
  isMeLoading = false;

  constructor(
    private _postService: PostService,
    private _userService: UserService,
    private _dialog: MatDialog,
    private _keycloakService: KeycloakService
  ) {
    _postService.onPostCreated$.subscribe(post => this.posts.unshift(post));
  }

  async ngOnInit(): Promise<void> {
    this._fetchPosts();

    this.isAuthenticated = await this._keycloakService.isLoggedIn();
    if (this.isAuthenticated) {
      this._fetchMe();
    }
  }

  _fetchPosts() {
    this.isLoading = true;

    this._postService.search('')
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: response => {
          if (response.body) {
            this.posts = response.body;
          }
        },
        error: e => console.error(e)
      });
  }

  _fetchMe() {
    this.isMeLoading = true;

    this._userService.getMe()
      .pipe(finalize(() => this.isMeLoading = false))
      .subscribe({
        next: response => {
          if (response.body) {
            this.me = response.body;
          }
        },
        error: e => console.error(e)
      });
  }

  fleet(): void {
    this._dialog.open(PostCreateOrEditDialogComponent, {
      width: '578px'
    }).afterClosed().subscribe((response?: IPost) => {
      if (response) {
        this.posts.unshift(response);
      }
    });
  }
}
