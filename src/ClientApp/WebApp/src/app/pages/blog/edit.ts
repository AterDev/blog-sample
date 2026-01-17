
import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules } from 'src/app/share/shared-modules';
import { forkJoin, Observable } from 'rxjs';
import { PageList } from 'src/app/services/admin/models/perigon/page-list.model';
import { BlogUpdateDto } from 'src/app/services/admin/models/blog-mod/blog-update-dto.model';
import { BlogDetailDto } from 'src/app/services/admin/models/blog-mod/blog-detail-dto.model';
import { BlogCategoryItemDto } from 'src/app/services/admin/models/blog-mod/blog-category-item-dto.model';

@Component({
  selector: 'app-blog-edit',
  imports: [CommonFormModules, MatCheckboxModule, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions],
  templateUrl: './edit.html'
})
export class BlogEdit implements OnInit {

  i18nKeys = I18N_KEYS;

  form!: FormGroup;
  id?: string;

  categoryIdOptions: BlogCategoryItemDto[] = [];

  constructor(
    private fb: FormBuilder,
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogEdit>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private translate: TranslateService
  ) {
    this.buildForm();
    this.id = data?.id;
  }

  ngOnInit() {
    const requests: Observable<PageList<any>>[] = [];
    requests.push(this.getCategoryIds());
    forkJoin(requests).subscribe({
      next: (results) => {
        this.categoryIdOptions = results[0].data || [];
      }
    });
    if (this.id) {
      this.adminClient.blog.detail(this.id).subscribe((res: BlogDetailDto) => this.form.patchValue(res));
    }
  }

  getCategoryIds(): Observable<PageList<BlogCategoryItemDto>> {
    return this.adminClient.blogCategory.list({ pageIndex: 1, pageSize: 100 });
  }


  buildForm() {
    this.form = this.fb.group({
      "title": [null, [Validators.maxLength(200)]],
      "content": [null, [Validators.maxLength(50000)]],
      "categoryIds": [null, []]
    });
  }

  get title() { return this.form.get('title') as FormControl; }
  get content() { return this.form.get('content') as FormControl; }
  get categoryIds() { return this.form.get('categoryIds') as FormControl; }

  getValidatorMessage(control: AbstractControl | null): string {
    if (!control || !control.errors) { return ''; }
    const errors = control.errors;
    const key = Object.keys(errors)[0];
    const params = errors[key];
    return this.translate.instant(`validation.${key.toLowerCase()}`, params);
  }

  submit() {
    if (this.form.invalid) return;
    if (!this.id) return;
    this.adminClient.blog.update(this.id, this.form.value as BlogUpdateDto).subscribe(() => this.dialogRef.close(true));
  }

  close(result: boolean) { this.dialogRef.close(result); }
}