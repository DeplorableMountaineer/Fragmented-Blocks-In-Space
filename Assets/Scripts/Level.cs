using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private int num_breakable_blocks;
    // Start is called before the first frame update
    void Start() {
    }

    public int BreakBlock() {
        num_breakable_blocks--;
        return num_breakable_blocks;
    }

    public void AddBlock() {
        num_breakable_blocks++;
    }
}
