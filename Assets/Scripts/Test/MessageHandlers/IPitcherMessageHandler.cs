using UnityEngine.EventSystems;

namespace Test
{
  public interface IPitcherMessageHandler : IEventSystemHandler
  {
    void EnablePitch();
  }
}