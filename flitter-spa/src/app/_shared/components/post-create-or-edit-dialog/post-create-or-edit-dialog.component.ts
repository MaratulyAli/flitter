import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { finalize } from 'rxjs';
import { IPostCreateOrEditDialogData } from '../../interfaces/dialog-data.interface';
import { IOption, IPollCreate } from '../../models/poll.model';
import { IPostCreate, IPostUpdate } from '../../models/post.model';
import { PostService } from '../../services/post.service';

@Component({
  selector: 'app-post-create-or-edit-dialog',
  templateUrl: './post-create-or-edit-dialog.component.html',
  styleUrls: ['./post-create-or-edit-dialog.component.scss']
})
export class PostCreateOrEditDialogComponent implements OnInit {
  postForm: FormGroup;
  postFormControls: {
    text: AbstractControl
  };
  pollForm: FormGroup;
  pollFormControls: {
    expiresAt: AbstractControl,
    pollNotExpire: AbstractControl,
    options: FormArray
  };

  isBusy = false;
  isLoading = false;
  pollEnabled = false;

  constructor(
    private _fb: FormBuilder,
    private _postService: PostService,
    public dialogRef: MatDialogRef<PostCreateOrEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public dialogData: IPostCreateOrEditDialogData
  ) {
    this.pollForm = _fb.group({
      expiresAt: [null],
      pollNotExpire: [true],
      options: _fb.array([])
    })
    this.postForm = _fb.group({
      text: ['', [Validators.required, Validators.maxLength(1000)]],
      poll: this.pollForm
    });

    this.postFormControls = {
      text: this.postForm.controls.text
    };
    this.pollFormControls = {
      expiresAt: this.pollForm.controls.expiresAt,
      pollNotExpire: this.pollForm.controls.pollNotExpire,
      options: this.pollForm.controls.options as FormArray
    };
  }

  ngOnInit(): void {
    if (this.dialogData) {
      this._fetchPost();
    }
  }

  _fetchPost(): void {
    this.isLoading = true;

    this._postService.getById(this.dialogData.postId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: response => {
          const post = response.body;
          if (post) {
            this.postFormControls.text.setValue(post.text);

            if (post.poll) {
              this.pollEnabled = true;

              if (post.poll.expiresAt != null) {
                this.pollFormControls.expiresAt.setValue(post.poll.expiresAt);
                this.pollFormControls.pollNotExpire.setValue(false);
              } else {
                this.pollFormControls.pollNotExpire.setValue(true);
              }

              post.poll.options.forEach(option => {
                this.addOption(option);
              });
            }
          }
        }
      })
  }

  submit(): void {
    if (this.postForm.pristine) {
      this.dialogRef.close();
      return;
    }

    if (this.postForm.dirty && this.postForm.valid) {
      this.isBusy = true;

      if (this.dialogData) {
        this.editPost();
      } else {
        this.createPost();
      }
    }
  }

  createPost() {
    const poll: IPollCreate = {
      expiresAt: this.pollFormControls.pollNotExpire.value
        ? null
        : this.pollFormControls.expiresAt.value,
      options: this.pollFormControls.options.value
    };
    const post: IPostCreate = {
      text: this.postFormControls.text.value,
      poll: !this.pollEnabled ? null : poll
    };

    this._postService.create(post)
      .pipe(finalize(() => this.isBusy = false))
      .subscribe({
        next: response => {
          if (response.body) {
            this.dialogRef.close(response.body);
          }
        },
        error: e => console.error(e)
      });
  }

  editPost() {
    const poll: IPollCreate = {
      expiresAt: this.pollFormControls.pollNotExpire.value
        ? null
        : this.pollFormControls.expiresAt.value,
      options: this.pollFormControls.options.value
    };
    const post: IPostUpdate = {
      id: this.dialogData.postId,
      text: this.postFormControls.text.value,
      poll: !this.pollEnabled ? null : poll
    };

    this._postService.update(post)
      .pipe(finalize(() => this.isBusy = false))
      .subscribe({
        next: response => {
          if (response.body) {
            this.dialogRef.close(response.body);
          }
        },
        error: e => console.error(e)
      });
  }

  addOption(option?: IOption) {
    const optionGroup = this._fb.group({
      id: [option?.id || null],
      text: [option?.text || '', [Validators.required, Validators.maxLength(100)]]
    });

    this.pollFormControls.options.push(optionGroup);
  }

  removeOption(i: number) {
    this.pollFormControls.options.removeAt(i);
    this.postForm.markAsDirty();
  }

  addPoll() {
    this.pollEnabled = true;

    if (this.pollFormControls.options.length === 0) {
      this.addOption();
    }
  }

  removePoll() {
    this.pollEnabled = false;
    this.postForm.markAsDirty();
  }
}
