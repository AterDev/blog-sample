/**
 * 用户AddDto
 */
export interface UserAddDto {
  /** 用户名 */
  userName: string;
  /** 密码 */
  password: string;
  /** 昵称 */
  nickName?: string | null;
  /** 头像URL */
  avatar?: string | null;
}
