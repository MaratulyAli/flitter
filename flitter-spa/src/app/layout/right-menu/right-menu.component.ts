import { Component, OnInit } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { IUser } from 'src/app/_shared/models/user.model';

@Component({
  selector: 'app-right-menu',
  templateUrl: './right-menu.component.html',
  styleUrls: ['./right-menu.component.scss']
})
export class RightMenuComponent implements OnInit {
  isAuthenticated = false;
  usersToFollow: IUser[] = [];

  constructor(
    private keycloakService: KeycloakService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.isAuthenticated = await this.keycloakService.isLoggedIn();
  }

  followUser(userId: string) {
    console.log('userId', userId);
  }

  login() {
    this.keycloakService.login();
  }
}
