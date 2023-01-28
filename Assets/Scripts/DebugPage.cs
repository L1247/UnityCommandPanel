#region

using UnityEngine;

#endregion

namespace CheatTool
{
    public class DebugPage : PageBase
    {
    #region Unity events

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // foreach (var button in buttons)
                // {
                //     EventSystem.current.SetSelectedGameObject(button.gameObject);
                // }
            }
        }

    #endregion

    #region Protected Methods

        protected override void Initialization()
        {
            AddButton("Test1" , () => Debug.Log("1"));
            AddButton("Test2" , () => Debug.Log("2"));
            AddButton("Test3" , () => Debug.Log("3"));
            AddButton("Test4" , () => Debug.Log("4"));
        }

    #endregion
    }
}