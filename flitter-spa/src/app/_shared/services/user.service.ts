import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { IUser } from "../models/user.model";
import { RestService } from "./rest.service";

@Injectable({ providedIn: 'root' })
export class UserService {
  baseUrl = environment.api.users;

  constructor(private _restService: RestService) { }

  getMe() {
    return this._restService.get<IUser>(this.baseUrl + '/me');
  }
}
