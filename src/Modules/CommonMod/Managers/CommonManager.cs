using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Share.Exceptions;
using Share.Models;

namespace CommonMod.Managers;

public class CommonManager(ILogger<CommonManager> logger, Localizer localizer, IWebHostEnvironment environment) : ManagerBase(logger)
{
    private readonly IWebHostEnvironment _environment = environment;

    public Dictionary<string, List<EnumDictionary>> GetEnumDictionary()
    {
        var enums = EnumHelper.GetAllEnumInfo();

        enums
            .Values.ToList()
            .ForEach(v =>
            {
                v.ForEach(e =>
                {
                    e.Description = localizer?.Get(e.Description) ?? e.Description;
                });
            });

        return enums;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="directory">目录</param>
    /// <param name="allowedExtensions">允许的扩展名，不指定则允许所有</param>
    /// <returns>文件的访问URL</returns>
    public async Task<string> UploadFileAsync(IFormFile file, string directory, string[]? allowedExtensions = null)
    {
        if (file == null || file.Length == 0)
        {
            throw new BusinessException(Localizer.FileIsEmpty);
        }

        // 获取文件扩展名
        var extension = Path.GetExtension(file.FileName).ToLower();

        // 验证文件类型
        if (allowedExtensions != null && allowedExtensions.Length > 0)
        {
            var extensionWithoutDot = extension.TrimStart('.');
            if (!allowedExtensions.Contains(extensionWithoutDot, StringComparer.OrdinalIgnoreCase))
            {
                throw new BusinessException(Localizer.FileTypeNotAllowed);
            }
        }

        // 生成文件名
        var fileName = $"{Guid.CreateVersion7():N}{extension}";

        // 构建完整路径
        var uploadsPath = Path.Combine(_environment.WebRootPath, directory);
        
        // 确保目录存在
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var filePath = Path.Combine(uploadsPath, fileName);

        // 保存文件
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 返回相对URL
        var url = $"/{directory.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}";
        return url;
    }
}
