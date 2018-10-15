using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

namespace Gameflow
{
    // Testing script for Character System
    public class CharacterSystemTEST : MonoBehaviour
    {
        public CharacterManager characterManager;
        public CharacterData stick;

        IEnumerator Start()
        {
            AddCharacter add = ScriptableObject.CreateInstance(typeof(AddCharacter)) as AddCharacter;
            add.characterName = "Stick";
            add.pos = Vector2.zero;
            add.startingPose = "Normal";
            add.startingExpression = "Happy";
            characterManager.characterControl(add);
            yield return new WaitForSeconds(1f);

            MoveCharacter move = ScriptableObject.CreateInstance(typeof(MoveCharacter)) as MoveCharacter;
            move.characterName = "Sticky";
            move.targetPos = new Vector2(2, 0);
            characterManager.characterControl(move);
            yield return new WaitForSeconds(1f);

            SetExpression exp = ScriptableObject.CreateInstance(typeof(SetExpression)) as SetExpression;
            exp.characterName = "Stick";
            exp.expression = "Angry";
            characterManager.characterControl(exp);
            yield return new WaitForSeconds(1f);

            RemoveCharacter remove = ScriptableObject.CreateInstance(typeof(RemoveCharacter)) as RemoveCharacter;
            remove.characterName = "StickMan";
            characterManager.characterControl(remove);
        }
    }
}

