import { User } from '../entity/user.model';

/**
 * 博客ItemDto
 */
export interface BlogItemDto {
  /** id */
  id: string;
  /** createdTime */
  createdTime: Date;
  /** title */
  title: string;
  /** authorUserName */
  authorUserName: string;
  /** 用户 */
  user: User;
}
