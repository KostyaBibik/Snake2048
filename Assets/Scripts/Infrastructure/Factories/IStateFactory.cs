using Components.Boxes.States;
using Components.Boxes.Views.Impl;
using Enums;
using UnityEngine;

namespace Infrastructure.Factories
{
    public interface IStateFactory
    {
        IBoxState CreateFollowState(Transform leader);
        IBoxState CreateIdleState();
        IBoxState CreateMoveState();
        IBoxState CreateMergeState(BoxView boxToMerge, EBoxGrade targetGrade);
        IBoxState CreateBotMoveState();
    }
}