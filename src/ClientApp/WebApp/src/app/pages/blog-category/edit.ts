
import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules } from 'src/app/share/shared-modules';
import { BlogCategoryUpdateDto } from 'src/app/services/admin/models/blog-mod/blog-category-update-dto.model';
import { BlogCategoryDetailDto } from 'src/app/services/admin/models/blog-mod/blog-category-detail-dto.model';

@Component({
  selector: 'app-blogCategory-edit',
  imports: [CommonFormModules, MatCheckboxModule],
  templateUrl: './edit.html'
})
export class BlogCategoryEdit implements OnInit {

  i18nKeys = I18N_KEYS;

  form!: FormGroup;
  id?: string;

  constructor(
    private fb: FormBuilder,
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogCategoryEdit>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private translate: TranslateService
  ) {
    this.buildForm();
    this.id = data?.id;
  }

  ngOnInit() {
    if (this.id) {
      this.adminClient.blogCategory.detail(this.id).subscribe((res: BlogCategoryDetailDto) => this.form.patchValue(res));
    }
  }

  buildForm() {
    this.form = this.fb.group({
      "name": [null, [Validators.maxLength(60)]],
      "description": [null, [Validators.maxLength(500)]]
    });
  }

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
    this.adminClient.blogCategory.update(this.id, this.form.value as BlogCategoryUpdateDto).subscribe(() => this.dialogRef.close(true));
  }

  close(result: boolean) { this.dialogRef.close(result); }
}