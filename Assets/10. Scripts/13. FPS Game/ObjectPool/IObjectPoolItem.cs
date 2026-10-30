using UnityEngine;

namespace Study3D.FpsGame
{
    // 저번 팀플에 썼던 Fish다
    public interface IObjectPoolItem
    {
        string Key { get; set; }
        GameObject GameObject { get; }

        void ReturnToPool();
    }
}
