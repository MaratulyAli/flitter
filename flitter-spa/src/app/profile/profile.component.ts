import { Component, OnInit } from '@angular/core';
import { IPost } from '../_shared/models/post.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  posts: IPost[] = [];

  constructor() { }

  ngOnInit(): void {
  }

}
