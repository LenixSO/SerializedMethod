using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class ListField<T, TValue> : VisualElement where T : BaseField<TValue>
{
    private Label emptyList;
    private VisualElement content;
    public Button plusButton { get; private set; }
    public Button minusButton { get; private set; }

    private float contentSize => (elementSize * Mathf.Max(count, 1)) + elementSpacing * 6;

    private List<ListElement<T, TValue>> elementList = new();
    private IntegerField countField;
    private List<ListElement<T, TValue>> selectedElements = new();
    private bool interactable = true;

    public int count => elementList.Count;
    public bool Interactable => interactable;

    #region Preset
    //Style parameters
    private readonly float elementSize = 25;
    public static readonly float elementSpacing = 1.2f;
    public static readonly Color bgColor = ColorExtension.GrayShade(.26f);
    public static readonly Color lighterColor = ColorExtension.GrayShade(.2f);
    public static readonly Color darkerColor = ColorExtension.GrayShade(.4f);
    public static readonly Color borderColor = ColorExtension.GrayShade(.1f);
    public static readonly float borderSize = 1.1f;
    public static readonly float borderRadius = 3f;
    public static readonly float buttonPaddingHorizontal = 4f;
    public static readonly float buttonPaddingVertical = 2f;

    private static Button ListButtons(Texture image)
    {
        Button button = new();
        button.style.flexGrow = 1;
        button.SetBorder(0, default);
        button.SetMargin(0);
        button.SetPadding(buttonPaddingHorizontal);
        button.style.paddingTop = buttonPaddingVertical;
        button.style.paddingBottom = buttonPaddingVertical;
        button.SetClickEffect(bgColor, darkerColor, lighterColor, TrickleDown.TrickleDown);
        Image iconPlus = new();
        iconPlus.image = image;
        iconPlus.style.flexGrow = 1;
        button.Add(iconPlus);

        return button;
    }

    private void SetupDragDrop(ListElement<T, TValue> element)
    {
        bool dragging = false;
        float totalArea = (element.localBound.height * (elementList.Count - 1)) + (element.localBound.height / 2f);
        Vector3 initialPosition = Vector3.zero;

        int originIndex = 0;
        int currentIndex = 0;
        int indexOffset = 0;//if already shifted up or down
        float shiftOffset = .75f;
        float shiftAnimationTime = .05f;

        //somehow it is blocking input, fix later
        //return;

        element.RegisterCallback<PointerDownEvent>((evt) =>
        {
            if (interactable)
            {
                if (element.field.Contains(evt.target as VisualElement)) return;
                //setup values
                totalArea = (element.localBound.height * (elementList.Count - 1)) + (element.localBound.height / 2f);
                initialPosition = element.transform.position;
                indexOffset = 0;
                originIndex = elementList.IndexOf(element);
                currentIndex = originIndex;
                dragging = true;

                //alter element
                element.BringToFront();
                element.CapturePointer(evt.pointerId);
                for (int i = 0; i < elementList.Count; i++)
                    elementList[i].pickingMode = PickingMode.Ignore;
            }

            bool containsElement = selectedElements.Contains(element);
            if (selectedElements.Count <= 0 || (!evt.ctrlKey && !evt.shiftKey))
            {
                ClearSelectedElements();
                selectedElements.Add(element);
                containsElement = true;
            }
            else if (evt.shiftKey)
            {
                selectedElements.Add(element);
                containsElement = true;
                
                //select all in between
                int minId = elementList.IndexOf(selectedElements[0]);
                int maxId = minId;

                for (int i = 1; i < selectedElements.Count; i++)
                {
                    int id = elementList.IndexOf(selectedElements[i]);
                    if (id < minId) minId = id;
                    if (id > maxId) maxId = id;
                }

                for (int i = minId; i < maxId; i++)
                {
                    if (selectedElements.Contains(elementList[i])) continue;

                    ListElement<T,TValue> currentElement = elementList[i];
                    selectedElements.Add(currentElement);
                    currentElement.SelectStyle(true);
                }
            }
            else
            {
                //ctrl pressed
                if (!containsElement)
                    selectedElements.Add(element);
                else
                    selectedElements.Remove(element);

                containsElement = !containsElement;
            }
            element.SelectStyle(containsElement);
        });

        element.RegisterCallback<PointerMoveEvent>((evt) =>
        {
            if(!interactable) return;
            if (!dragging) return;

            //set position
            float finalYPosition = Mathf.Clamp(evt.deltaPosition.y + element.transform.position.y, 0, totalArea);
            Vector2 position = element.transform.position;
            position.y = finalYPosition;
            element.transform.position = position;

            //mshiftAnimationTimeove other elements out of the way
            Vector3 indexPosition = ElementPosition(currentIndex);
            float distanceFromOrigin = position.y - indexPosition.y;
            //element below is 1 index above this
            if (distanceFromOrigin > elementSize * shiftOffset && currentIndex < count - 1)
                ShiftElements(1);
            //element above is 1 index below this
            else if (distanceFromOrigin < -(elementSize * shiftOffset) && currentIndex > 0)
                ShiftElements(-1);
            
            return;
            //A cycle between origin/current?
            //cycle direction changes
            void ShiftElements(int offset)
            {
                //move element
                int index = currentIndex + offset;
                if (indexOffset != (int)Mathf.Sign((offset))) index += indexOffset;//use only if going back to origin
                elementList[index].MoveTowards(indexPosition, shiftAnimationTime);

                //updateId
                currentIndex += offset;
                indexOffset = Mathf.Clamp(currentIndex - originIndex, -1, 1);
            }
        });

        element.RegisterCallback<PointerUpEvent>((evt) =>
        {
            if (interactable)
            {
                if (!dragging) return;
                Vector3 finalPosition = ElementPosition(currentIndex);
                element.transform.position = finalPosition;

                if (currentIndex != originIndex)
                {
                    //adjust indexes
                    indexOffset = Mathf.Clamp(currentIndex - originIndex, -1, 1);
                    elementList.Insert(currentIndex + Mathf.Clamp(indexOffset, 0, 1),
                        element); //insert at +1 if origin is before current
                    elementList.RemoveAt(originIndex -
                                         Mathf.Clamp(indexOffset, -1, 0)); //remove at +1 if origin is after current

                    //create change list
                    int min = Mathf.Min(currentIndex, originIndex);
                    int max = Mathf.Max(currentIndex, originIndex);
                    Dictionary<int, ChangeEvent<TValue>> valuesChanged = new();
                    CheckChanges(min, max, -indexOffset, valuesChanged);

                    //adjust labels
                    UpdateLabels();

                    BroadCastChangeEvent(changed: valuesChanged);
                }

                //enable interaction again
                for (int i = 0; i < elementList.Count; i++) elementList[i].pickingMode = PickingMode.Position;
            }

            element.ReleasePointer(evt.pointerId);
            content.FocusElement(true);
            dragging = false;
        });
    }

    #endregion

    public ListField(string label = "ListField")
    {
        VisualElement header = new();
        header.style.flexDirection = FlexDirection.Column;

        Foldout foldout = new();
        foldout.text = label;
        foldout.style.flexGrow = 1;

        countField = new("");
        countField.style.width = 50;
        countField.style.height = 18;
        countField.style.position = Position.Absolute;
        countField.style.alignSelf = Align.FlexEnd;
        countField.RegisterCallback<NavigationSubmitEvent>(ElementCountChanged);

        content = new();
        content.style.backgroundColor = bgColor;
        content.SetBorder(borderSize, borderRadius, borderColor);
        //resizeElementsAtStart
        content.RegisterCallback<GeometryChangedEvent>(ContentResize);
        void ContentResize(GeometryChangedEvent evt) { ResizeContent(); }

        VisualElement buttonsWindow = new();
        buttonsWindow.SetBorder(borderSize, borderRadius, borderColor);
        buttonsWindow.style.flexDirection = FlexDirection.Row;
        buttonsWindow.style.borderTopWidth = 0;
        buttonsWindow.style.marginRight = 15;
        buttonsWindow.style.width = 55;
        buttonsWindow.style.height = 22;
        buttonsWindow.style.backgroundColor = bgColor;
        buttonsWindow.style.alignSelf = Align.FlexEnd;

        plusButton = ListButtons(EditorGUIUtility.IconContent("Toolbar Plus").image);
        plusButton.name = "Plus Button";
        plusButton.clicked += AddNewElement;
        minusButton = ListButtons(EditorGUIUtility.IconContent("Toolbar Minus").image);
        minusButton.name = "Minus Button";
        minusButton.clicked += RemoveElement;

        buttonsWindow.Add(plusButton);
        buttonsWindow.Add(minusButton);

        emptyList = new("List is empty");
        emptyList.SetMargin(5);
        emptyList.style.marginBottom = 2;
        content.Add(emptyList);
        
        foldout.Add(content);
        foldout.Add(buttonsWindow);
        header.Add(foldout);
        header.Add(countField);
        
        content.RegisterCallback<BlurEvent>(evt =>
        {
            if(evt.relatedTarget == minusButton) return;
            //clear selected elements
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].SelectStyle(false);
            }
            selectedElements.Clear();
        });
        Add(header);
        ResizeContent();
    }

    private void ElementCountChanged(NavigationSubmitEvent evt)
    {
        int difference = countField.value - count;
        if(difference == 0) return;
        //Debug.Log($"cound changed by {difference}");
        
        Dictionary<int, TValue> added = new();
        Dictionary<int, TValue> removed = new();
        if (difference < 0)
        {
            //remove elements
            difference *= -1;
            int removeCount = Mathf.Min(difference, elementList.Count);
            for (int i = 0; i < removeCount; i++)
            {
                var element = elementList[^1];
                RemoveTargetElement(element);
                removed[count] = element.Value;
            }
        }
        else
        {
            //add elements
            for (int i = 0; i < difference; i++)
            {
                added[elementList.Count] = BaseAddElement().value;
            }
        }
        
        UpdateListData();
        UpdateLabels();
        ResizeContent();
        BroadCastChangeEvent(added: added, removed: removed);
    }

    private void AddNewElement()
    {
        ClearSelectedElements();
        AddElement();
    }

    private void RemoveElement()
    {
        Dictionary<int, TValue> removed = new();
        Dictionary<int, ChangeEvent<TValue>> changed = new();
        if (selectedElements.Count <= 0)
        {
            if (elementList.Count > 0)
            {
                var element = elementList[^1];
                RemoveTargetElement(element);
                removed[count] = element.Value;
            }
        }
        else
        {
            List<int> indexes = new(selectedElements.Count);
            for (int i = 0; i < selectedElements.Count; i++)
                indexes.Add(elementList.IndexOf(selectedElements[i]));
            
            indexes.Sort();
            for (int i = 0; i < indexes.Count; i++)
            {
                int id = indexes[i];
                CheckChanges(id, count - 1, -1, changed);
                changed.Remove(id);//first id not needed
            }

            for (int i = 0; i < selectedElements.Count; i++)
            {
                var element = selectedElements[i];
                RemoveTargetElement(element);
                removed[count] = element.Value;
            }

            selectedElements.Clear();
        }

        UpdateListData();
        UpdateLabels();
        ResizeContent();
        BroadCastChangeEvent(removed: removed, changed: changed);
    }
    
    private void RemoveTargetElement(ListElement<T, TValue> element)
    {
        element.UnregisterCallback<ChangeEvent<TValue>>(ElementValueChanged);
        content.Remove(element);
        elementList.Remove(element);
    }

    public void ClearList()
    {
        Dictionary<int, TValue> removed = new();
        int elementCount = elementList.Count;
        for (int i = 0; i < elementCount; i++)
        {
            var element = elementList[0];
            removed[i] = element.Value;
            RemoveTargetElement(element);
        }
        ResizeContent();
        BroadCastChangeEvent(removed: removed);
    }

    private T BaseAddElement(TValue value = default)
    {
        ListElement<T, TValue> element = new($"Element {count}");
        element.style.position = Position.Absolute;
        element.style.height = elementSize - elementSpacing;
        element.field.value = value;
        element.RegisterCallback<ChangeEvent<TValue>>(ElementValueChanged);
        element.SelectStyle(false);
        elementList.Add(element);
        content.Add(element);
        ResizeContent();
        SetupDragDrop(element);
        UpdateListData();
        return element.field;
    }
    
    public T AddElement(TValue value = default)
    {
        var field = BaseAddElement(value);
        BroadCastChangeEvent(added: new Dictionary<int, TValue>() { { count - 1, value } });
        return field;
    }

    public TValue ValueAt(int id)
    {
        if (id < 0 || id >= count) return default;
        return elementList[id].Value;
    }

    public T FieldAt(int id)
    {
        if (id < 0 || id >= count) return null;
        return elementList[id].field;
    }

    public TValue[] GetValues()
    {
        TValue[] values = new TValue[count];
        for (int i = 0; i < values.Length; i++)
            values[i] = elementList[i].Value;
        return values;
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
        
        countField.SetEnabled(value);
        plusButton.SetEnabled(value);
        minusButton.SetEnabled(value);
    }

    private void UpdateLabels()
    {
        for (int i = 0; i < elementList.Count; i++)
            elementList[i].field.label = $"Element {i}";
    }
    private void ResizeContent()
    {
        //resize conten to fit all elements
        content.style.height = contentSize;
        float horizontalSpacing = content.style.borderLeftWidth.value + content.style.borderRightWidth.value;
        horizontalSpacing += content.style.paddingLeft.value.value + content.style.paddingRight.value.value;
        horizontalSpacing += content.style.marginLeft.value.value + content.style.marginRight.value.value;

        //position elements
        for (int i = 0; i < count; i++)
        {
            int index = count - 1 - i;
            ListElement<T, TValue> element = elementList[index];
            element.transform.position = ElementPosition(index);
            element.style.width = content.worldBound.width - horizontalSpacing;
        }
    }

    private void ClearSelectedElements()
    {
        for (int i = 0; i < selectedElements.Count; i++)
            selectedElements[i].SelectStyle(false);
        selectedElements.Clear();
    }

    private Vector3 ElementPosition(int index) => Vector3.up * ((contentSize - elementSize * (count - 1 - index)) - (elementSize + elementSpacing * 4));

    private void ElementValueChanged(ChangeEvent<TValue> evt)
    {
        UpdateListData();
        if (evt.currentTarget is ListElement<T, TValue> element)
        {
            int id = elementList.IndexOf(element);
            BroadCastChangeEvent(changed: new Dictionary<int, ChangeEvent<TValue>>() { { id, evt } });
        }
    }

    private void UpdateListData()
    {
        countField.SetValueWithoutNotify(count);
        emptyList.style.display = count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void BroadCastChangeEvent(
        Dictionary<int, TValue> removed = null,
        Dictionary<int, TValue> added = null,
        Dictionary<int, ChangeEvent<TValue>> changed = null)
    {
        var evt = CollectionChangeEvent<TValue>.GetPooled();
        evt.target = this;

        //add changes
        evt.removedValues = removed ?? new();
        evt.addedValues = added ?? new();
        evt.changedValues = changed ?? new();

        SendEvent(evt);
    }

    /// <param name="cycleDirection">1 = added; -1 = removed</param>
    private void CheckChanges(int min, int max, int cycleDirection, 
        Dictionary<int, ChangeEvent<TValue>> valuesChanged)
    {
        valuesChanged ??= new();
        int size = max - min + 1;
        for (int i = min; i <= max; i++)
        {
            //cycling using Rest division to determine old index
            int oldIndex = min + ((i - min) + size + cycleDirection) % size;
            //Debug.Log($"{i} was {oldIndex}");
            ChangeEvent<TValue> change =
                ChangeEvent<TValue>.GetPooled(elementList[oldIndex].Value, elementList[i].Value);
            valuesChanged[i] = change;
        }
    }
}

public class ListElement<T, TValue> : VisualElement where T : BaseField<TValue>
{
    public T field {  get; private set; }

    public TValue Value
    {
        get => field.value;
        set => field.value = value;
    }

    private Image image;

    private readonly float borderSize = .1f;
    private readonly Color bgColor = new(.25f,.25f,.25f);
    private readonly Color defaultColor = new(.75f,.75f,.75f);
    private readonly Color selectedColor = new(0.4f, 0.7f, 0.8f);

    public ListElement(string label = "ListElement", TValue value = default)
    {
        style.flexDirection = FlexDirection.Row;
        //style.marginTop = ListField<T, TValue>.elementSpacing;
        style.paddingLeft = 5f;
        style.paddingRight = 5f;
        this.SetBorder(borderSize, borderColor: bgColor);
        this.SetClickEffect(ListField<T, TValue>.bgColor, ListField<T, TValue>.lighterColor, ListField<T, TValue>.darkerColor);

        image = new();
        image.image = EditorGUIUtility.IconContent("align_vertically_bottom").image;

        field = Activator.CreateInstance<T>();
        field.label = label;
        field.style.flexGrow = 1;
        field.SetValueWithoutNotify(value);

        Add(image);
        Add(field);
    }

    public void SelectStyle(bool selected)
    {
        Color color = selected ? selectedColor : defaultColor;
        this.SetBorder(borderSize, borderColor: selected ? selectedColor : bgColor);
        image.tintColor = color;
        field.labelElement.style.color = color;
    }
}

public class CollectionChangeEvent<TValue> : EventBase<CollectionChangeEvent<TValue>>
{
    public Dictionary<int, ChangeEvent<TValue>> changedValues = new();
    public Dictionary<int, TValue> addedValues = new();
    public Dictionary<int, TValue> removedValues = new();
}