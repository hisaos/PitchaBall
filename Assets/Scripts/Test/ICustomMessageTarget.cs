using UnityEngine.EventSystems;

namespace Test
{
  public interface ICustomMessageTarget : IEventSystemHandler
  {
    void EnablePitch();
  }
}