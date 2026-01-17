
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules, CommonListModules } from 'src/app/share/shared-modules';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { forkJoin, Observable } from 'rxjs';
import { PageList } from 'src/app/services/admin/models/perigon/page-list.model';
import { ConfirmDialogComponent } from 'src/app/share/components/confirm-dialog/confirm-dialog.component';
import { BlogFilterDto } from 'src/app/services/admin/models/blog-mod/blog-filter-dto.model';
import { BlogItemDto } from 'src/app/services/admin/models/blog-mod/blog-item-dto.model';
import { BlogAdd } from './add';
import { BlogEdit } from './edit';
import { BlogDetail } from './detail';
import { BlogCategoryItemDto } from 'src/app/services/admin/models/blog-mod/blog-category-item-dto.model';

@Component({
  selector: 'app-blog-index',
  imports: [CommonListModules, CommonFormModules, MatAutocompleteModule],
  templateUrl: './index.html'
})
export class BlogIndex implements OnInit {

  i18nKeys = I18N_KEYS;

  filterDto: BlogFilterDto = { pageIndex: 1, pageSize: 10 };
  dataSource = new MatTableDataSource<BlogItemDto>();
  displayedColumns = [ "Id", "CreatedTime", "Title", "AuthorUserName", "actions" ];

  total = 0;
  pageSize = 10;
  categoryIdOptions: BlogCategoryItemDto[] = [];

  constructor(
    private adminClient: AdminClient,
    private dialog: MatDialog,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const requests: Observable<PageList<any>>[] = [];
    requests.push(this.getCategoryId());
    forkJoin(requests).subscribe({
      next: (results) => {
        this.categoryIdOptions = results[0].data || [];
      }
    });
    this.loadData();
  }

  getCategoryId(): Observable<PageList<BlogCategoryItemDto>> {
    return this.adminClient.blogCategory.list({ pageIndex: 1, pageSize: 100 });
  }


  loadData(): void {
    this.adminClient.blog.list(this.filterDto as BlogFilterDto).subscribe((res) => {
      this.dataSource.data = (res.data || []);
      this.total = (res.count ?? res.data?.length ?? this.dataSource.data.length);
    });
  }

  filter(): void {
    this.filterDto.pageIndex = 1;
    this.loadData();
  }

  pageChanged(e: any) {
    this.filterDto.pageIndex = e.pageIndex + 1;
    this.filterDto.pageSize = e.pageSize;
    this.loadData();
  }

  openAdd() {
    const ref = this.dialog.open(BlogAdd, { width: '800px' });
    ref.afterClosed().subscribe((r: boolean) => { if (r) this.loadData(); });
  }

  openEdit(id: string) {
    const ref = this.dialog.open(BlogEdit, { width: '800px', data: { id } });
    ref.afterClosed().subscribe((r: boolean) => { if (r) this.loadData(); });
  }

  openDetail(id: string) {
    this.dialog.open(BlogDetail, { minWidth: '600px', data: { id } });
  }

  deleteItem(id: string) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.translate.instant('common.confirm'),
        content: this.translate.instant('common.deleteConfirm')
      }
    });
    ref.afterClosed().subscribe((ok: boolean) => {
      if (ok) { this.adminClient.blog.delete(id).subscribe(() => this.loadData()); }
    });
  }
}