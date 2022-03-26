namespace BaiduPanCompareTools.vo.diff
{
    internal enum DiffStateEnum
    {
        None,   // 某文件夹在较老、较新快照中均存在，但仅标识此文件夹，不涉及子文件夹、子文件的变更状态
        Add,    // 某文件夹或文件仅在较新快照中存在，说明是新的中新增的
        Delete, // 某文件夹或文件仅在较老快照中存在，说明是新的中已删除的
        Modity, // 某文件在较老、较新快照中均存在，但文件MD5值变化，说明发生了内容修改
    }
}
