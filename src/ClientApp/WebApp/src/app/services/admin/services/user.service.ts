import { BaseService } from '../base.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginDto } from '../models/user-mod/login-dto.model';
import { AccessTokenDto } from '../models/share/access-token-dto.model';
import { UserAddDto } from '../models/user-mod/user-add-dto.model';
import { User } from '../models/entity/user.model';
import { UserUpdateDto } from '../models/user-mod/user-update-dto.model';
import { UserDetailDto } from '../models/user-mod/user-detail-dto.model';
/**
 * 用户
 */
@Injectable({ providedIn: 'root' })
export class UserService extends BaseService {
  /**
   * 用户登录 ✅
   * @param data LoginDto
   */
  login(data: LoginDto): Observable<AccessTokenDto> {
    const _url = `/api/User/login`;
    return this.request<AccessTokenDto>('post', _url, data);
  }
  /**
   * add
   * @param data UserAddDto
   */
  add(data: UserAddDto): Observable<User> {
    const _url = `/api/User`;
    return this.request<User>('post', _url, data);
  }
  /**
   * Update 用户 ✅
   * @param id
   * @param data UserUpdateDto
   */
  update(id: string, data: UserUpdateDto): Observable<boolean> {
    const _url = `/api/User/${id}`;
    return this.request<boolean>('patch', _url, data);
  }
  /**
   * Get 用户 Detail ✅
   */
  detail(): Observable<UserDetailDto> {
    const _url = `/api/User/detail`;
    return this.request<UserDetailDto>('get', _url);
  }
  /**
   * 上传用户头像 ✅
   * @param id 用户ID
   * @param data any
   */
  uploadAvatar(id: string, data: any): Observable<string> {
    const _url = `/api/User/${id}/avatar`;
    return this.request<string>('post', _url, data);
  }
}