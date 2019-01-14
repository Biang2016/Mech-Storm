using System.IO;
using System;

public abstract class DownloadItem
{
    public UpdateManager.DownloadFileInfo DownloadFileInfo;

    /// <summary>
    /// 网络资源url路径
    /// </summary>
    protected string m_srcUrl;
    /// <summary>
    /// 资源下载存放路径，不包含文件名
    /// </summary>
    protected string m_savePath;
    /// <summary>
    /// 文件名,不包含后缀
    /// </summary>
    protected string m_fileNameWithoutExt;
    /// <summary>
    /// 文件后缀
    /// </summary>
    protected string m_fileExt;
    /// <summary>
    /// 下载文件全路径，路径+文件名+后缀
    /// </summary>
    protected string m_saveFilePath;
    /// <summary>
    /// 原文件大小
    /// </summary>
    protected long m_fileLength;
    /// <summary>
    /// 当前下载好了的大小
    /// </summary>
    protected long m_currentLength;
    /// <summary>
    /// 是否开始下载
    /// </summary>
    protected bool m_isStartDownload;
    public bool isStartDownload
    {
        get
        {
            return m_isStartDownload;
        }
    }

    public DownloadItem(string url, string path, UpdateManager.DownloadFileInfo downloadFileInfo = null)
    {
        DownloadFileInfo = downloadFileInfo;
        m_srcUrl = url;
        m_savePath = path;
        m_isStartDownload = false;
        m_fileNameWithoutExt = Path.GetFileNameWithoutExtension(m_srcUrl);
        m_fileExt = Path.GetExtension(m_srcUrl);
        m_saveFilePath = string.Format("{0}/{1}{2}", m_savePath, m_fileNameWithoutExt, m_fileExt);
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    /// <param name="callback">下载完成回调</param>
    public virtual void StartDownload(Action callback = null)
    {
        if (string.IsNullOrEmpty(m_srcUrl) || string.IsNullOrEmpty(m_savePath))
        {
            return;
        }
        //若存放目录不存在则创建目录
        FileUtils.CreateDirectory(m_saveFilePath);
    }

    /// <summary>
    /// 获取下载进度
    /// </summary>
    /// <returns>进度，0-1</returns>
    public abstract float GetProcess();

    /// <summary>
    /// 获取当前下载了的文件大小
    /// </summary>
    /// <returns>当前文件大小</returns>
    public abstract long GetCurrentLength();

    /// <summary>
    /// 获取要下载的文件大小
    /// </summary>
    /// <returns>文件大小</returns>
    public abstract long GetLength();

    public abstract void Destroy();
}
