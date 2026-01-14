/**
 * 用户
 */
export interface User {
  /** id */
  id: string;
  /** createdTime */
  createdTime: Date;
  /** updatedTime */
  updatedTime: Date;
  /** isDeleted */
  isDeleted: boolean;
  /** tenantId */
  tenantId: string;
  /** 用户名 */
  userName: string;
  /** 密码 */
  passwordHash: string;
  /** salt */
  salt: string;
  /** 昵称 */
  nickName?: string | null;
  /** 头像URL */
  avatar?: string | null;
}
