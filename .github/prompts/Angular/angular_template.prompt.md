# angular_template

生成Angular前端页面代码模板内容，使用razor语法.

使用最新的angulr material组件库。直接使用组件样式，不需要自定义样式。

内容要包含多语言的支持，使用最新的signals特性(包括最新的signal forms)，后端请求通过注入`AdminClient`进行调用，

如当前是用户模块，则使用`this.adminClient.user.xxx`进行调用：

- 添加是调用`this.adminClient.user.add(dto:UserAddDto)`
- 编辑是调用`this.adminClient.user.update(id:string,dto:UserUpdateDto)`
- 获取详情是调用`this.adminClient.user.detail(id:string)`
- 获取列表是调用`this.adminClient.user.filter(filterDto:UserFilterDto)`
- 删除是调用`this.adminClient.user.delete(id:string)`

对应的类型分别是：

- 列表筛选使用`UserFilterDto`
- 列表项使用`UserItemDto`
- 添加使用`UserAddDto`
- 编辑使用`UserUpdateDto`
- 详情使用`UserDetailDto`

通常组件包含.ts和.html两个文件，根据以下情况进行生成：

## 列表页面

列表由筛选组件(toolbar)和table组件组成。

- 筛选组件通过`XXXFilterDto`中包含的字段生成对应的筛选组件，如字符串使用输入框，枚举使用下拉框，布尔值使用开关等。添加按钮(add组件)放在筛选行的最右侧。
- 表格组件通过`XXXItemDto`中的字段生成对应的列，包含操作列，支持分页功能。操作列包含查看详情(detail/{id})，编辑(edit)，删除按钮
- 删除要求弹出确认对话框，使用已经封装好的组件`ConfirmDialogComponent`。
- 编辑默认也使用弹窗的方式调用编辑组件`edit`。
- 添加时弹窗调用添加组件`add`，添加成功后刷新列表。

## 添加页面

- 同时支持弹窗和路由两种方式打开。根据是否有弹窗的数据决定。
- 根据`XXXAddDto`生成对应的表单组件。并实现添加功能，添加成功后关闭弹窗或跳转回列表页面。
- 要有完整的表单验证，根据`XXXAddDto`中的验证特性生成对应的验证规则。

## 编辑页面
- 同时支持弹窗和路由两种方式打开。根据是否有弹窗的数据决定。
- 要先根据id获取详情数据进行回填，id通过路由参数或弹窗数据传入。
- 根据`XXXUpdateDto`生成对应的表单组件。并实现编辑功能，编辑成功后关闭弹窗或跳转回列表页面。
- 要有完整的表单验证，根据`XXXUpdateDto`中的验证特性生成对应的验证规则。

## 详情页面

- 同时支持弹窗和路由两种方式打开。根据是否有弹窗的数据决定。
- 根据id获取详情数据进行展示，id通过路由参数或弹窗数据传入。
- 根据`XXXDetailDto`生成对应的详情展示组件。
- 包含返回按钮，返回列表页面。

## 表单内容说明

- .ts和html要能对应上，可借助get 方法简化模板代码。
- 表单使用最新的signal forms特性。
- 可借助 validation-helpers.ts 来简化验证规则的编写。