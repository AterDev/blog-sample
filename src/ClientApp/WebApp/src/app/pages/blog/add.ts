
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules } from 'src/app/share/shared-modules';
import { forkJoin, Observable } from 'rxjs';
import { PageList } from 'src/app/services/admin/models/perigon/page-list.model';
import { BlogAddDto } from 'src/app/services/admin/models/blog-mod/blog-add-dto.model';
import { BlogCategoryItemDto } from 'src/app/services/admin/models/blog-mod/blog-category-item-dto.model';

@Component({
  selector: 'app-blog-add',
  imports: [CommonFormModules, MatCheckboxModule, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions],
  templateUrl: './add.html'
})
export class BlogAdd implements OnInit {

  i18nKeys = I18N_KEYS;

  form!: FormGroup;

  categoryIdOptions: BlogCategoryItemDto[] = [];

  constructor(
    private fb: FormBuilder,
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogAdd>,
    private translate: TranslateService
  ) {
    this.buildForm();
  }

  ngOnInit(): void {
    const requests: Observable<PageList<any>>[] = [];
    requests.push(this.getCategoryIds());
    forkJoin(requests).subscribe({
      next: (results) => {
        this.categoryIdOptions = results[0].data || [];
      }
    });
  }

  getCategoryIds(): Observable<PageList<BlogCategoryItemDto>> {
    return this.adminClient.blogCategory.list({ pageIndex: 1, pageSize: 100 });
  }


  buildForm() {
    this.form = this.fb.group({
      title: [null, [Validators.required, Validators.maxLength(200)]],
      content: [null, [Validators.required, Validators.maxLength(50000)]],
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
    this.adminClient.blog.add(this.form.value as BlogAddDto).subscribe(() => this.dialogRef.close(true));
  }

  close(result: boolean) { this.dialogRef.close(result); }
}