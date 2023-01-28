#region

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

namespace CheatTool
{
    public class PageBase : MonoBehaviour
    {
    #region Private Variables

        private readonly List<ButtonCellModel> buttonCellModels = new List<ButtonCellModel>();
        private readonly List<ButtonCellModel> cellsForSearch   = new List<ButtonCellModel>();

        private readonly List<UnityEngine.UI.Selectable> selectables = new List<UnityEngine.UI.Selectable>();

        private TMP_InputField searchField;

        [SerializeField]
        private Button buttonPrefab;

        [SerializeField]
        private TMP_InputField inputFieldPrefab;

        [SerializeField]
        private Transform content;

    #endregion

    #region Unity events

        protected virtual void Start()
        {
            AddSearchField("type something");
            Initialization();
            InitializationAfter();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (EventSystem.current.currentSelectedGameObject != searchField.gameObject)
                    Select(searchField.gameObject);
        }

    #endregion

    #region Protected Methods

        protected void AddButton(string cellText , Action clicked = null)
        {
            var button          = Instantiate(buttonPrefab , content);
            var buttonCellModel = button.gameObject.AddComponent<ButtonCellModel>();
            buttonCellModel.Button = button;
            buttonCellModel.Name   = cellText;

            buttonCellModels.Add(buttonCellModel);

            var tmpText = button.GetComponentInChildren<TMP_Text>();
            tmpText.text = cellText;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => clicked?.Invoke());
            selectables.Add(button);
        }

        protected virtual void Initialization() { }

    #endregion

    #region Private Methods

        private void AddSearchField(string placeholder)
        {
            searchField = Instantiate(inputFieldPrefab , content);
            var placeholderTextComponent = searchField.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();
            placeholderTextComponent.text = placeholder;
            selectables.Add(searchField);
        }

        private void InitializationAfter()
        {
            SetNavigationOfSelects();
            SelectFirst();
            searchField.onValueChanged.AddListener(str =>
                                                   {
                                                       cellsForSearch.Clear();
                                                       foreach (var buttonCellModel in buttonCellModels)
                                                       {
                                                           var containKeyWord = buttonCellModel.Name.Contains(str);
                                                           if (containKeyWord)
                                                           {
                                                               cellsForSearch.Add(buttonCellModel);
                                                               buttonCellModel.gameObject.SetActive(true);
                                                           }
                                                           else
                                                           {
                                                               buttonCellModel.gameObject.SetActive(false);
                                                           }
                                                       }

                                                       Debug.Log($"ValueChanged: {str}");
                                                   });
            searchField.onEndEdit.AddListener(str => Debug.Log($"EndEdit: {str}"));
        }

        private void Select(GameObject gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void SelectFirst()
        {
            var firstSelectable = selectables[0].gameObject;
            Select(firstSelectable);
        }

        private void SetNavigationOfSelects()
        {
            var count = selectables.Count;
            for (var index = 0 ; index < count ; index++)
            {
                int upIndex;
                int downIndex;

                var selectableObj = selectables[index];
                selectableObj.gameObject.AddComponent<Selectable>();
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

                var up   = selectables[upIndex];
                var down = selectables[downIndex];

                selectableObj.navigation = new Navigation { mode = Navigation.Mode.Explicit , selectOnUp = up , selectOnDown = down };
            }
        }

    #endregion
    }
}