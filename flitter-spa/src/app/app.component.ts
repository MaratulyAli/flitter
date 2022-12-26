import { Component, OnInit } from '@angular/core';
import { KeycloakEventType, KeycloakService } from 'keycloak-angular';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  env = environment;
  isLoading = true;
  type: KeycloakEventType = KeycloakEventType.OnTokenExpired;

  constructor(private a: KeycloakService) {
    a.keycloakEvents$.subscribe(e => {
      console.log(KeycloakEventType[e.type]);
    })
  }

  async ngOnInit(): Promise<void> {
    this.isLoading = false;

  }
}
