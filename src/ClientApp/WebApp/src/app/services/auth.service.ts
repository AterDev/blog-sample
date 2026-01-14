import { Injectable } from '@angular/core';
import { AccessTokenDto } from './admin/models/share/access-token-dto.model';
import { UserDetailDto } from './admin/models/user-mod/user-detail-dto.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  isLogin = false;
  isAdmin = false;
  userName?: string | null = null;
  id?: string | null = null;
  constructor() {
    this.updateUserLoginState();
  }

  saveToken(token: AccessTokenDto): void {
    this.isLogin = true;
    localStorage.setItem("accessToken", token.accessToken);
    localStorage.setItem("refreshToken", token.refreshToken);

  }

  saveUserInfo(userinfo: UserDetailDto): void {
    this.isLogin = true;
    this.userName = userinfo.userName;
    localStorage.setItem("username", userinfo.userName ?? '');
  }

  updateUserLoginState(): void {
    const username = localStorage.getItem('username');
    const token = localStorage.getItem('accessToken');
    if (token && username) {
      this.userName = username;
      this.isLogin = true;
    } else {
      this.isLogin = false;
    }
  }
  logout(): void {
    localStorage.clear();
    this.isLogin = false;
  }
}
