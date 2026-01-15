import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules } from 'src/app/share/shared-modules';
import { BlogAddDto } from 'src/app/services/admin/models/blog-mod/blog-add-dto.model';

@Component({
  selector: 'app-blog-add',
  imports: [CommonFormModules],
  templateUrl: './add.html'
})
export class BlogAdd {

  i18nKeys = I18N_KEYS;

  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private adminClient: AdminClient,
    private dialogRef: MatDialogRef<BlogAdd>,
    private translate: TranslateService
  ) {
    this.buildForm();
  }

  buildForm() {
    this.form = this.fb.group({
      title: [null, [Validators.required, Validators.maxLength(200)]],
      content: [null, [Validators.required, Validators.maxLength(50000)]],
      authorId: [null]
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
    this.adminClient.blog.add(this.form.value as BlogAddDto).subscribe(() => this.dialogRef.close(true));
  }

  close(result: boolean) { this.dialogRef.close(result); }
}