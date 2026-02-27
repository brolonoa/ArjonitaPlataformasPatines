using UnityEngine;

public interface IParryable 
{
    public void OnParry();
    bool CanBeParried { get; }
    

}
