import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ILikeCreate } from '../models/like.model';
import { IOption, IOptionVoteCreate } from '../models/poll.model';
import { IPost, IPostCreate, IPostUpdate } from '../models/post.model';
import { RestService } from './rest.service';

@Injectable({ providedIn: 'root' })
export class PostService {
  baseUrl = environment.api.posts;
  onPostCreated$ = new Subject<IPost>();

  constructor(private _restService: RestService) { }

  search(query: string) {
    return this._restService.get<IPost[]>(this.baseUrl + '/search', { query });
  }

  getById(id: number) {
    return this._restService.get<IPost>(this.baseUrl + '/' + id);
  }

  getByUserId(userId: string) {
    return this._restService.get<IPost[]>(this.baseUrl + '/users/' + userId);
  }

  create(body: IPostCreate) {
    return this._restService.create<IPost>(this.baseUrl, body);
  }

  update(body: IPostUpdate) {
    return this._restService.update<IPost>(this.baseUrl, body);
  }

  delete(id: number) {
    return this._restService.delete(this.baseUrl + '/' + id);
  }

  like(body: ILikeCreate) {
    return this._restService.create(this.baseUrl + '/likes', body);
  }

  vote(body: IOptionVoteCreate) {
    return this._restService.create<IOption[]>(this.baseUrl + '/polls/vote', body);
  }
}
