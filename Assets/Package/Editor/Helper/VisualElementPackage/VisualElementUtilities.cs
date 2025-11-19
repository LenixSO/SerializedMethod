using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.UIElements.PointerType;

public static class VisualElementUtilities
{
    #region GemeralVisualElement
    public static VisualElement GetRoot(this VisualElement visualElement)
    {
        VisualElement root = visualElement;
        while (root.parent != null) root = root.parent;
        return root;
    }

    public static void SetPadding(this VisualElement visualElement, float borderWidth)
    {
        visualElement.style.paddingTop = borderWidth;
        visualElement.style.paddingBottom = borderWidth;
        visualElement.style.paddingLeft = borderWidth;
        visualElement.style.paddingRight = borderWidth;
    }

    public static void SetMargin(this VisualElement visualElement, float borderWidth)
    {
        visualElement.style.marginTop = borderWidth;
        visualElement.style.marginBottom = borderWidth;
        visualElement.style.marginLeft = borderWidth;
        visualElement.style.marginRight = borderWidth;
    }

    public static void SetBorder(this VisualElement visualElement, float borderWidth, float curvature = 0, Color? borderColor = null)
    {
        //size
        visualElement.style.borderTopWidth = borderWidth;
        visualElement.style.borderBottomWidth = borderWidth;
        visualElement.style.borderLeftWidth = borderWidth;
        visualElement.style.borderRightWidth = borderWidth;

        //color
        Color color = borderColor ?? visualElement.style.backgroundColor.value;
        visualElement.style.borderTopColor = color;
        visualElement.style.borderBottomColor = color;
        visualElement.style.borderLeftColor = color;
        visualElement.style.borderRightColor = color;

        //curvature
        visualElement.style.borderTopLeftRadius = curvature;
        visualElement.style.borderTopRightRadius = curvature;
        visualElement.style.borderBottomLeftRadius = curvature;
        visualElement.style.borderBottomRightRadius = curvature;
    }

    #endregion

    public static void SetClickEffect(this VisualElement element, Color defaultColor, Color? hoverColor = null, Color? clickColor = null,
        TrickleDown trickleDownClick = TrickleDown.NoTrickleDown)
    {
        Color normalColor = defaultColor;
        Color highlightColor = hoverColor ?? defaultColor;
        Color downColor = clickColor ?? defaultColor;

        element.style.backgroundColor = normalColor;

        element.RegisterCallback<MouseEnterEvent>((evt) =>
        {
            if (element.pickingMode == PickingMode.Ignore || !element.enabledSelf) return;
            //Debug.Log("mouse enter");
            element.style.backgroundColor = highlightColor;
        });
        element.RegisterCallback<MouseLeaveEvent>((evt) =>
        {
            if (element.pickingMode == PickingMode.Ignore || !element.enabledSelf) return;
            //Debug.Log($"mouse leave: {evt.pressedButtons}");
            bool holdingButton = evt.pressedButtons > 0;
            if (holdingButton && evt.currentTarget != element) return;
            element.style.backgroundColor = normalColor;
        });

        element.RegisterCallback<MouseDownEvent>((evt) =>
        {
            if (element.pickingMode == PickingMode.Ignore || !element.enabledSelf) return;
            element.CaptureMouse();
            element.style.backgroundColor = downColor;
            //Debug.Log("mouse down");
        }, trickleDownClick);
        element.RegisterCallback<MouseUpEvent>((evt) =>
        {
            if (element.pickingMode == PickingMode.Ignore || !element.enabledSelf) return;
            element.ReleaseMouse();
            bool mouseOver = element.ContainsPoint(evt.localMousePosition);
            element.style.backgroundColor = mouseOver ? highlightColor : normalColor;
            //Debug.Log($"mouse up: {element.ContainsPoint(evt.localMousePosition)}");
        });
    }
    
    /// <summary>
    /// apparently only works (and is needed) on editor visualElement
    /// </summary>
    public static void FocusElement(this VisualElement element, bool forceFocusable = false)
    {
        if (!element.focusable)
        {
            if (!forceFocusable)
            {
                Debug.LogWarning($"Element [{element}] is not focusable");
                return;
            }
            element.focusable = true;
        }

        bool isRoot = element.parent == null;
        VisualElement root = element;

        while (!isRoot)
        {
            root = root.parent;
            isRoot = root.parent == null;
        }

        Focusable lastFocused = root.panel.focusController.focusedElement;
        FocusOutEvent focusOutEvent = FocusEventBase<FocusOutEvent>.GetPooled(lastFocused, lastFocused, FocusChangeDirection.none, root.panel.focusController);
        FocusEvent focusEvent = FocusEventBase<FocusEvent>.GetPooled(element, lastFocused, FocusChangeDirection.unspecified, root.panel.focusController);

        root.SendEvent(focusOutEvent);
        root.SendEvent(focusEvent);
    }

    #region ScrollView
    public static void EnableMouseDrag(this ScrollView scrollView)
    {
        VisualElement root = scrollView.GetRoot();
        bool dragging = false;
        Vector2 maxMovement = Vector2.zero;
        scrollView.RegisterCallback<PointerDownEvent>((evt) =>
        {
            dragging = true;
            maxMovement = scrollView.contentContainer.contentRect.size - scrollView.contentRect.size;
        });
        root.RegisterCallback<PointerMoveEvent>((evt) =>
        {
            if (!dragging || evt.pointerType != PointerType.mouse) return;
            VisualElement content = scrollView.contentContainer;

            //scroll mode
            Vector3 delta = evt.deltaPosition;
            if (scrollView.mode == ScrollViewMode.Horizontal) delta.y = 0f;
            else if (scrollView.mode == ScrollViewMode.Vertical) delta.x = 0f;

            //clamp drag
            Vector3 position = content.transform.position + delta;
            position.x = Mathf.Clamp(position.x, 0, -maxMovement.x);
            position.y = Mathf.Clamp(position.y, -maxMovement.y, 0);

            content.transform.position = position;
        });
        root.RegisterCallback<PointerUpEvent>((evt) =>
        {
            dragging = false;
        });
    }
    public static void IndicateMoreContent(this ScrollView scrollView, float transparency)
    {
        VisualElement root = scrollView.GetRoot();
        bool dragging = false;
        Vector2 maxMovement = Vector2.zero;
        scrollView.style.borderTopWidth = 10;
        scrollView.style.borderBottomWidth = 10;
        scrollView.style.borderBottomColor = ColorExtension.TransparentBlack(transparency);
        scrollView.RegisterCallback<PointerDownEvent>((evt) =>
        {
            dragging = true;
            maxMovement = scrollView.contentContainer.contentRect.size - scrollView.contentRect.size;
        });
        root.RegisterCallback<PointerMoveEvent>((evt) =>
        {
            if (!dragging || evt.pointerType != PointerType.mouse) return;

            //scroll mode
            Vector3 delta = evt.deltaPosition;
            if (scrollView.mode == ScrollViewMode.Horizontal) delta.y = 0f;
            else if (scrollView.mode == ScrollViewMode.Vertical) delta.x = 0f;

            if (scrollView.contentContainer.transform.position.y < 0)
            {
                //top shade
                scrollView.style.borderTopColor = ColorExtension.TransparentBlack(transparency);
            }
            else
            {
                scrollView.style.borderTopColor = ColorExtension.TransparentBlack(0);
            }

            if (scrollView.contentContainer.transform.position.y > -maxMovement.y)
            {
                //top shade
                scrollView.style.borderBottomColor = ColorExtension.TransparentBlack(transparency);
            }
            else
            {
                scrollView.style.borderBottomColor = ColorExtension.TransparentBlack(0);
            }

        });
        root.RegisterCallback<PointerUpEvent>((evt) =>
        {
            dragging = false;
        });
    }
    #endregion

    #region Animations
    public static void MoveTowards(this VisualElement element, Vector3 goal, float animationDuration, Action onDone = null)
    {
        Vector3 startPosition = element.transform.position;
        float step = .02f;
        float scaledStep = (step / animationDuration);

        float time = 0;
        var animation = element.schedule.Execute(() => MoveTowards(goal));
        animation.Every((long)(step * 1000));
        animation.Until(AnimationDone);

        void MoveTowards(Vector3 position)
        {
            element.transform.position = Vector3.Lerp(startPosition, position, time);
            time += scaledStep;
            if (AnimationDone()) onDone?.Invoke();
        }

        bool AnimationDone() => time > 1 + scaledStep;
    }
    #endregion

    #region Info

    //transitions
    //https://discussions.unity.com/t/announcing-uss-transition/863464
    //https://discussions.unity.com/t/quick-transition-tutorial/863467/3
    //element.style.transitionProperty = new List<StylePropertyName>() { new ("background-color") };
    //element.style.transitionDuration = new List<TimeValue> { new (1.0f, TimeUnit.Second) };
    #endregion
}
