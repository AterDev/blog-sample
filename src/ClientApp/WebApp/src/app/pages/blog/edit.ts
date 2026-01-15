import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AdminClient } from 'src/app/services/admin/admin-client';
import { I18N_KEYS } from 'src/app/share/i18n-keys';
import { CommonFormModules } from 'src/app/share/shared-modules';
import { BlogUpdateDto } from 'src/app/services/admin/models/blog-mod/blog-update-dto.model';
import { BlogDetailDto } from 'src/app/services/admin/models/blog-mod/blog-detail-dto.model';

@Component({
  selector: 'app-blog-edit',
  imports: [CommonFormModules],
  templateUrl: './edit.html'
})
export class BlogEdit implements OnInit {

  i18nKeys = I18N_KEYS;

  form!: FormGroup;
  id?: string;

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
    if (this.id) {
      this.adminClient.blog.detail(this.id).subscribe((res: BlogDetailDto) => this.form.patchValue(res));
    }
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
    if (!this.id) return;
    this.adminClient.blog.update(this.id, this.form.value as BlogUpdateDto).subscribe(() => this.dialogRef.close(true));
  }

  close(result: boolean) { this.dialogRef.close(result); }
}