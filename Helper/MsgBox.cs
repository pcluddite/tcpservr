using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace Tcpservr {
    public class MsgBox {

        public MsgBoxStyle Style { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }

        public MsgBox(int flag, string text, string title) {
            this.Text = text;
            this.Title = title;
            this.Style = MsgBoxStyle.OkOnly;
            if ((flag & 1) == 1) { this.Style = this.Style | MsgBoxStyle.OkCancel; }
            if ((flag & 2) == 2) { this.Style = this.Style | MsgBoxStyle.AbortRetryIgnore; }
            if ((flag & 3) == 3) { this.Style = this.Style | MsgBoxStyle.YesNoCancel; }
            if ((flag & 4) == 4) { this.Style = this.Style | MsgBoxStyle.YesNo; }
            if ((flag & 5) == 5) { this.Style = this.Style | MsgBoxStyle.RetryCancel; }
            if ((flag & 16) == 16) { this.Style = this.Style | MsgBoxStyle.Critical; }
            if ((flag & 32) == 32) { this.Style = this.Style | MsgBoxStyle.Question; }
            if ((flag & 48) == 48) { this.Style = this.Style | MsgBoxStyle.Exclamation; }
            if ((flag & 64) == 64) { this.Style = this.Style | MsgBoxStyle.Information; }
            if ((flag & 256) == 256) { this.Style = this.Style | MsgBoxStyle.DefaultButton2; }
            if ((flag & 512) == 512) { this.Style = this.Style | MsgBoxStyle.DefaultButton3; }
            if ((flag & 262144) == 262144) { this.Style = this.Style | MsgBoxStyle.MsgBoxSetForeground; }
            if ((flag & 524288) == 524288) { this.Style = this.Style | MsgBoxStyle.MsgBoxRight; }
        }

        public MsgBoxResult Show() {
            return Interaction.MsgBox(this.Text, this.Style, this.Title);
        }
    }
}
