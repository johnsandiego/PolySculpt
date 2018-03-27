using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class FlexibleUIButton : FlexibleUI
{

    protected Button button;
    protected Image image;

    public ButtonType buttonType;

    public enum ButtonType
    {
        Default,
        Confirm,
        Decline,
        Warning
    }

    public override void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        base.Awake();
    }

    protected override void OnSkinUI()
    {
        button.transition = Selectable.Transition.ColorTint;
        button.targetGraphic = image;

        image.sprite = skinData.buttonSprite;
        image.type = Image.Type.Sliced;
        button.spriteState = skinData.buttonSpriteState;

        switch (buttonType)
        {
            case ButtonType.Confirm:
                image.color = skinData.confirmColor;
                break;
            case ButtonType.Decline:
                image.color = skinData.declineColor;
                break;
            case ButtonType.Default:
                image.color = skinData.defaultColor;
                break;
            case ButtonType.Warning:
                image.color = skinData.warningColor;
                break;
        }


        base.OnSkinUI();
    }



}