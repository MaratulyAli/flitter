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
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  env = environment;
  isAuthenticated = false;
  me!: IUser;
  isLoadingUser = false;

  constructor(
    private dialog: MatDialog,
    private keycloakService: KeycloakService,
    private postService: PostService,
    private userService: UserService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.isAuthenticated = await this.keycloakService.isLoggedIn();
    console.log('this.isAuthenticated', this.isAuthenticated);

    if (this.isAuthenticated) {
      this.isLoadingUser = true;

      this.userService.getMe()
        .pipe(finalize(() => this.isLoadingUser = false))
        .subscribe({
          next: response => {
            if (response.body) {
              this.me = response.body;
            }
          },
          error: e => console.error(e)
        })
    }
  }

  fleet() {
    this.dialog.open(PostCreateOrEditDialogComponent, {
      width: '578px'
    }).afterClosed().subscribe((response?: IPost) => {
      if (response) {
        this.postService.onPostCreated$.next(response);
      }
    });
  }

  logout() {
    this.keycloakService.logout();
  }
}
