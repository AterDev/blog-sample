
import { Component, Inject, OnInit, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatListModule } from '@angular/material/list';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { BaseMatModules } from 'src/app/share/shared-modules';
import { BlogCategoryDetailDto } from 'src/app/services/admin/models/blog-mod/blog-category-detail-dto.model';

@Component({
  selector: 'app-blogCategory-detail',
  imports: [BaseMatModules, MatListModule, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions],
  templateUrl: './detail.html'
})
export class BlogCategoryDetail implements OnInit {

  i18nKeys = I18N_KEYS;

  model!: BlogCategoryDetailDto;
  id?: string;
  isLoading = signal(true);

  constructor(
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogCategoryDetail>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.id = data?.id;
  }

  ngOnInit() {
    if (this.id) {
      this.adminClient.blogCategory.detail(this.id).subscribe((res: BlogCategoryDetailDto) => {
        this.model = res;
        this.isLoading.set(false);
      });
    }
  }

  close() { this.dialogRef.close(); }
}