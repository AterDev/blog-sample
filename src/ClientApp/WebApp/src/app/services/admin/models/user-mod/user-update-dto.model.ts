/**
 * 用户UpdateDto
 */
export interface UserUpdateDto {
  /** password */
  password?: string | null;
  /** 昵称 */
  nickName?: string | null;
  /** 头像URL */
  avatar?: string | null;
}
