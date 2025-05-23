﻿using System;


namespace Template.AppLib;

public class Logical : AbstractValueHolder<bool> {
    public Logical() { }

    public Logical(bool value) : base(value) { }

    public Logical(bool value, Action<bool> callback) : base(value, callback) { }

    public override object Parse(string entry) {
        if ("true".StartsWith(entry.ToLower())) {
            return new Logical(true);
        }

        if ("false".StartsWith(entry.ToLower())) {
            return new Logical(false);
        }

        throw new Exception(entry);
    }

    
}