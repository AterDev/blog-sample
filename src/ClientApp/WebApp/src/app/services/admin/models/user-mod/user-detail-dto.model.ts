/**
 * 用户DetailDto
 */
export interface UserDetailDto {
  /** 用户名 */
  userName?: string | null;
  /** 昵称 */
  nickName?: string | null;
  /** 头像URL */
  avatar?: string | null;
  /** id */
  id: string;
  /** createdTime */
  createdTime: Date;
  /** updatedTime */
  updatedTime: Date;
}
