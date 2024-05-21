using Components.Boxes.States;
using Enums;
using UnityEngine;
using Views.Impl;

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