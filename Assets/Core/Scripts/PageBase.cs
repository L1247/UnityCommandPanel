#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

namespace rStart.UnityCommandPanel
{
    public class PageBase : MonoBehaviour
    {
    #region Private Variables

        private readonly List<ButtonCellModel> buttonCellModels = new List<ButtonCellModel>();
        private readonly List<ButtonCellModel> cellsForSearch   = new List<ButtonCellModel>();

        private readonly List<UnityEngine.UI.Selectable> selectables = new List<UnityEngine.UI.Selectable>();

        private CanvasGroup canvasGroup;

        private GameObject buttonPrefab;

        private RectTransform content;

        private PrefabContainer prefabContainer;
        private bool            init;

    #endregion

    #region Public Methods

        public void AddButton(string cellText , string description = "" , Action clicked = null)
        {
            var button          = Instantiate(buttonPrefab , content).GetComponent<Button>();
            var buttonCellModel = button.gameObject.AddComponent<ButtonCellModel>();
            buttonCellModel.Button      = button;
            buttonCellModel.CellText    = cellText;
            buttonCellModel.Description = description;

            buttonCellModels.Add(buttonCellModel);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => clicked?.Invoke());
            selectables.Add(button);
            cellsForSearch.Add(buttonCellModel);
        }

        public void AddPageLinkButton<TPage>(string text) where TPage : PageBase
        {
            AddButton(text , $"Open {text} page" , () =>
                                                   {
                                                       CommandPanel.Instance.AddOrOpenPage<TPage>();
                                                   });
        }

        public void Init()
        {
            if (init) return;
            init            = true;
            prefabContainer = GetComponent<PrefabContainer>();
            content         = transform.parent.GetComponent<RectTransform>();
            buttonPrefab    = prefabContainer.GetPrefab("Button");
            Initialization();
            InitializationAfter();
        }

    #endregion

    #region Protected Methods

        protected virtual void Initialization() { }

    #endregion

    #region Private Methods

        private void ExecuteButtonOfSelectable(int index)
        {
            if (cellsForSearch.Count <= index) return;
            var selectable = cellsForSearch[index].Button;
            Select(selectable);
            ExecuteEvents.Execute(selectable.gameObject , new BaseEventData(EventSystem.current) , ExecuteEvents.submitHandler);
        }

        private void InitializationAfter()
        {
            SetNavigationOfSelects(selectables);
            foreach (var cellModel in selectables) cellModel.GetComponent<Selectable>().onSelect += OnSelected;
        }

        private void OnSelected(RectTransform selectable)
        {
            var description                                                            = string.Empty;
            if (selectable.TryGetComponent(out ButtonCellModel cellModel)) description = cellModel.Description;
            CommandPanel.Instance.SetDescriptionText(description);

            SnapTo(selectable);
        }

        private void Select(UnityEngine.UI.Selectable selectable)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }

        private void SetNavigationOfSelects(List<UnityEngine.UI.Selectable> selectableList)
        {
            var count = selectableList.Count;
            for (var index = 0 ; index < count ; index++)
            {
                int upIndex;
                int downIndex;

                var selectableObj = selectableList[index];
                if (selectableObj.gameObject.GetComponent<Selectable>() is null) selectableObj.gameObject.AddComponent<Selectable>();
                var isFirstCell = index == 0;
                var isLastCell  = index == count - 1;
                if (isFirstCell)
                {
                    upIndex   = count - 1;
                    downIndex = index + 1;
                }
                else if (isLastCell)
                {
                    upIndex   = index - 1;
                    downIndex = 0;
                }
                else
                {
                    upIndex   = index - 1;
                    downIndex = index + 1;
                }

                var maxNumber       = 9;
                var executionNumber = index > maxNumber ? 0 : index;
                if (selectableObj.TryGetComponent<ButtonCellModel>(out var buttonCellModel))
                    buttonCellModel.SetExecutionNumber(executionNumber);

                var countIsOne            = downIndex == count;
                if (countIsOne) downIndex = 0;

                var up   = selectableList[upIndex];
                var down = selectableList[downIndex];

                selectableObj.navigation = new Navigation { mode = Navigation.Mode.Explicit , selectOnUp = up , selectOnDown = down };
            }
        }

        private void SetPageVisible(bool visible)
        {
            CommandPanel.Instance.SetPageVisible(visible);
        }

        private void SnapTo(RectTransform target)
        {
            var buttonCellModel = target.GetComponent<ButtonCellModel>();
            var findIndex       = cellsForSearch.FindIndex(model => model == buttonCellModel);
            var objHeight       = target.rect.height;

            const int padding              = 100;
            var       localPositionY       = findIndex * objHeight - padding;
            var       contentLocalPosition = new Vector2(content.localPosition.x , localPositionY);

            content.localPosition = contentLocalPosition;
        }

    #endregion
    }
}