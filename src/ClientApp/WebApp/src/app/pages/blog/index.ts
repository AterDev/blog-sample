import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules, CommonListModules } from 'src/app/share/shared-modules';
import { ConfirmDialogComponent } from 'src/app/share/components/confirm-dialog/confirm-dialog.component';
import { BlogFilterDto } from 'src/app/services/admin/models/blog-mod/blog-filter-dto.model';
import { BlogItemDto } from 'src/app/services/admin/models/blog-mod/blog-item-dto.model';
import { BlogAdd } from './add';
import { BlogEdit } from './edit';
import { BlogDetail } from "./detail";
import { TypedCellDefDirective } from 'src/app/share/typed-cell-def.directive';
import { PageEvent } from '@angular/material/paginator';
import { forkJoin, Observable } from 'rxjs';
import { BlogCategoryItemDto } from 'src/app/services/admin/models/blog-mod/blog-category-item-dto.model';
import { PageList } from 'src/app/services/admin/models/perigon/page-list.model';

@Component({
  selector: 'app-blog-index',
  imports: [CommonListModules, CommonFormModules, TypedCellDefDirective],
  templateUrl: './index.html'
})
export class BlogIndex implements OnInit {
  i18nKeys = I18N_KEYS;
  filter: BlogFilterDto = { pageIndex: 1, pageSize: 10 };
  dataSource = new MatTableDataSource<BlogItemDto>();
  displayedColumns = ['title', 'authorId', 'createdTime', 'actions'];
  categoryOptions: BlogCategoryItemDto[] = [];
  total = 0;

  constructor(
    private adminClient: AdminClient,
    private dialog: MatDialog,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.reload();

    forkJoin([this.getCategoryId()])
      .subscribe({
        next: ([categoryRes]) => {
          this.categoryOptions = categoryRes.data || [];
        }
      });
  }

  getCategoryId(): Observable<PageList<BlogCategoryItemDto>> {
    return this.adminClient.blogCategory.list({ pageIndex: 1, pageSize: 100 });
  }



  reload(): void {
    this.adminClient.blog.list(this.filter).subscribe((res) => {
      this.dataSource.data = (res.data || []);
      this.total = (res.count ?? res.data?.length ?? this.dataSource.data.length);
    });
  }

  pageChanged(e: PageEvent) {
    this.filter.pageIndex = e.pageIndex + 1;
    this.filter.pageSize = e.pageSize;
    this.reload();
  }

  openAdd() {
    const ref = this.dialog.open(BlogAdd, { width: '800px' });
    ref.afterClosed().subscribe((r: boolean) => { if (r) this.reload(); });
  }

  openEdit(id: string) {
    const ref = this.dialog.open(BlogEdit, { width: '800px', data: { id } });
    ref.afterClosed().subscribe((r: boolean) => { if (r) this.reload(); });
  }

  openDetail(id: string) {
    this.dialog.open(BlogDetail, { width: '800px', data: { id } });
  }

  deleteItem(id: string) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.translate.instant('common.confirm'),
        content: this.translate.instant('common.deleteConfirm')
      }
    });
    ref.afterClosed().subscribe((ok: boolean) => {
      if (ok) { this.adminClient.blog.delete(id).subscribe(() => this.reload()); }
    });
  }
}