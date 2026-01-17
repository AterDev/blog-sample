
import { Component, Inject, OnInit, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatListModule } from '@angular/material/list';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { BaseMatModules } from 'src/app/share/shared-modules';
import { BlogDetailDto } from 'src/app/services/admin/models/blog-mod/blog-detail-dto.model';

@Component({
  selector: 'app-blog-detail',
  imports: [BaseMatModules, MatListModule, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions],
  templateUrl: './detail.html'
})
export class BlogDetail implements OnInit {

  i18nKeys = I18N_KEYS;

  model!: BlogDetailDto;
  id?: string;
  isLoading = signal(true);

  constructor(
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogDetail>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.id = data?.id;
  }

  ngOnInit() {
    if (this.id) {
      this.adminClient.blog.detail(this.id).subscribe((res: BlogDetailDto) => {
        this.model = res;
        this.isLoading.set(false);
      });
    }
  }

  close() { this.dialogRef.close(); }
}