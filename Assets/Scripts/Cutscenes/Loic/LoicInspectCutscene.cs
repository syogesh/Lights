﻿using UnityEngine;
using System.Collections.Generic;

public class LoicInspectCutscene : Cutscene {

    public string text;
    public float inspectionTime = 3f;
    public List<GameObject> objects;

    protected override void DefineCutscene() {
        if (text != "") {
            Do(duration: 2f, action: () => NaviSay(this.text));
        }

        foreach (var go in objects) {
            // Position *MUST* be stored outside of action closure here!
            var position = go.transform.position;

            Do(duration: this.inspectionTime, action: () => LockCamera(position));
        }
    }
}
