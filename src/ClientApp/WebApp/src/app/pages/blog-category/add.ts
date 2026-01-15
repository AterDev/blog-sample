
import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules } from 'src/app/share/shared-modules';
import { BlogCategoryAddDto } from 'src/app/services/admin/models/blog-mod/blog-category-add-dto.model';

@Component({
  selector: 'app-blogCategory-add',
  imports: [CommonFormModules, MatCheckboxModule],
  templateUrl: './add.html'
})
export class BlogCategoryAdd {

  i18nKeys = I18N_KEYS;

  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogCategoryAdd>,
    private translate: TranslateService
  ) {
    this.buildForm();
  }

  buildForm() {
    this.form = this.fb.group({
      "name": [null, [Validators.required, Validators.maxLength(60)]],
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
    this.adminClient.blogCategory.add(this.form.value as BlogCategoryAddDto).subscribe(() => this.dialogRef.close(true));
  }

  close(result: boolean) { this.dialogRef.close(result); }
}