
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules, CommonListModules } from 'src/app/share/shared-modules';
import { ConfirmDialogComponent } from 'src/app/share/components/confirm-dialog/confirm-dialog.component';
import { BlogCategoryFilterDto } from 'src/app/services/admin/models/blog-mod/blog-category-filter-dto.model';
import { BlogCategoryItemDto } from 'src/app/services/admin/models/blog-mod/blog-category-item-dto.model';
import { BlogCategoryAdd } from './add';
import { BlogCategoryEdit } from './edit';
import { BlogCategoryDetail } from './detail';

@Component({
  selector: 'app-blogCategory-index',
  imports: [CommonListModules, CommonFormModules],
  templateUrl: './index.html'
})
export class BlogCategoryIndex implements OnInit {

  i18nKeys = I18N_KEYS;

  filter: BlogCategoryFilterDto = { pageIndex: 1, pageSize: 10 };
  dataSource = new MatTableDataSource<BlogCategoryItemDto>();
  displayedColumns = [ "Name", "Id", "CreatedTime", "actions" ];

  total = 0;
  pageSize = 10;

  constructor(
    private adminClient: AdminClient,
    private dialog: MatDialog,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.adminClient.blogCategory.list(this.filter).subscribe((res: any) => {
      this.dataSource.data = (res.data || []);
      this.total = (res.count ?? res.data?.length ?? this.dataSource.data.length);
    });
  }

  pageChanged(e: any) {
    this.filter.pageIndex = e.pageIndex + 1;
    this.filter.pageSize = e.pageSize;
    this.reload();
  }

  openAdd() {
    const ref = this.dialog.open(BlogCategoryAdd, { width: '800px' });
    ref.afterClosed().subscribe((r: boolean) => { if (r) this.reload(); });
  }

  openEdit(id: string) {
    const ref = this.dialog.open(BlogCategoryEdit, { width: '800px', data: { id } });
    ref.afterClosed().subscribe((r: boolean) => { if (r) this.reload(); });
  }

  openDetail(id: string) {
    this.dialog.open(BlogCategoryDetail, { width: '800px', data: { id } });
  }

  deleteItem(id: string) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.translate.instant('common.confirm'),
        content: this.translate.instant('common.deleteConfirm')
      }
    });
    ref.afterClosed().subscribe((ok: boolean) => {
      if (ok) { this.adminClient.blogCategory.delete(id).subscribe(() => this.reload()); }
    });
  }
}