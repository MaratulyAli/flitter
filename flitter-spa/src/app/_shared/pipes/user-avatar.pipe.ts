import { Pipe, PipeTransform } from '@angular/core';
import { environment } from 'src/environments/environment';

@Pipe({
  name: 'userAvatar'
})
export class UserAvatarPipe implements PipeTransform {
  env = environment

  transform(value: string | null): string {
    return value ? value : this.env.assets.images.user;
  }

}
