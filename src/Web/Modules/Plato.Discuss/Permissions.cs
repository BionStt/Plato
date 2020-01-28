﻿using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Discuss
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostTopics =
            new Permission("PostTopics", "Post topics");

        public static readonly Permission PostReplies =
            new Permission("PostReplies", "Post replies");
        
        public static readonly Permission EditOwnTopics =
            new Permission("EditOwnTopics", "Edit own topics");

        public static readonly Permission EditAnyTopic =
            new Permission("EditAnyTopic", "Edit any topic");

        public static readonly Permission EditOwnReplies =
            new Permission("EditOwnReplies", "Edit own replies");

        public static readonly Permission EditAnyReply =
            new Permission("EditAnyReply", "Edit any reply");
        
        public static readonly Permission DeleteOwnTopics = 
            new Permission("DeleteOwnTopics", "Soft delete own topics");

        public static readonly Permission RestoreOwnTopics =
            new Permission("RestoreOwnTopics", "Restore own soft deleted topics");

        public static readonly Permission PermanentDeleteOwnTopics =
            new Permission("PermanentDeleteOwnTopics", "Permanently delete own topics");

        public static readonly Permission DeleteAnyTopic =
            new Permission("DeleteAnyTopic", "Soft delete any topic");

        public static readonly Permission RestoreAnyTopic =
            new Permission("RestoreAnyTopic", "Restore any soft deleted topic");

        public static readonly Permission PermanentDeleteAnyTopic =
            new Permission("PermanentDeleteAnyTopic", "Permanently delete any topic");

        public static readonly Permission ViewDeletedTopics =
            new Permission("ViewDeletedTopics", "View soft deleted topics");

        public static readonly Permission DeleteOwnReplies =
            new Permission("DeleteOwnReplies", "Soft delete own replies");

        public static readonly Permission RestoreOwnReplies =
            new Permission("RestoreOwnReplies", "Restore own soft deleted replies");

        public static readonly Permission PermanentDeleteOwnReplies =
            new Permission("PermanentDeleteOwnReplies", "Permanently delete own replies");

        public static readonly Permission DeleteAnyReply =
            new Permission("DeleteAnyReply", "Soft delete any reply");
        
        public static readonly Permission RestoreAnyReply =
            new Permission("RestoreAnyReply", "Restore any soft deleted reply");

        public static readonly Permission ViewDeletedReplies =
            new Permission("ViewDeletedReplies", "View soft deleted replies");

        public static readonly Permission PermanentDeleteAnyReply =
            new Permission("PermanentDeleteAnyReply", "Permanently delete any reply");

        public static readonly Permission ReportTopics =
            new Permission("ReportTopics", "Report topics");

        public static readonly Permission ReportReplies =
            new Permission("ReportReplies", "Report replies");
        
        public static readonly Permission PinTopics =
            new Permission("PinTopics", "Pin topics");

        public static readonly Permission UnpinTopics =
            new Permission("UnpinTopics", "Unpin topics");

        public static readonly Permission LockTopics =
            new Permission("LockTopics", "Lock topics");

        public static readonly Permission UnlockTopics =
            new Permission("UnlockTopics", "Unlock topics");

        public static readonly Permission HideTopics =
            new Permission("HideTopics", "Hide topics");

        public static readonly Permission ShowTopics =
            new Permission("ShowTopics", "Unhide topics");

        public static readonly Permission ViewHiddenTopics =
            new Permission("ViewHiddenTopics", "View hidden topics");

        public static readonly Permission ViewPrivateTopics =
            new Permission("ViewPrivateTopics", "View private topics");

        public static readonly Permission HideReplies =
            new Permission("HideReplies", "Hide replies");

        public static readonly Permission ShowReplies =
            new Permission("ShowReplies", "Unhide replies");
        
        public static readonly Permission ViewHiddenReplies =
            new Permission("ViewHiddenReplies", "View hidden replies");

        public static readonly Permission TopicToSpam =
            new Permission("TopicsToSpam", "Move topics to SPAM");

        public static readonly Permission TopicFromSpam =
            new Permission("TopicsFromSpam", "Remove topics from SPAM");

        public static readonly Permission ViewSpamTopics =
            new Permission("ViewSpamTopics", "View topics flagged as SPAM");
        
        public static readonly Permission ReplyToSpam =
            new Permission("RepliesToSpam", "Move replies to SPAM");

        public static readonly Permission ReplyFromSpam =
            new Permission("RepliesFromSpam", "Remove replies from SPAM");

        public static readonly Permission ViewSpamReplies =
            new Permission("ViewSpamReplies", "View replies flagged as SPAM");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostTopics,
                PostReplies,
                EditOwnTopics,
                EditAnyTopic,
                EditOwnReplies,
                EditAnyReply,
                DeleteOwnTopics,
                RestoreOwnTopics,
                PermanentDeleteOwnTopics,
                DeleteAnyTopic,
                RestoreAnyTopic,
                PermanentDeleteAnyTopic,
                ViewDeletedTopics,
                DeleteOwnReplies,
                RestoreOwnReplies,
                PermanentDeleteOwnReplies,
                DeleteAnyReply,
                RestoreAnyReply,
                PermanentDeleteAnyReply,
                ViewDeletedReplies,
                ReportTopics,
                ReportReplies,
                PinTopics,
                UnpinTopics,
                LockTopics,
                UnlockTopics,
                HideTopics,
                ShowTopics,
                ViewHiddenTopics,
                ViewPrivateTopics,
                HideReplies,
                ShowReplies,
                ViewHiddenReplies,
                TopicToSpam,
                TopicFromSpam,
                ViewSpamTopics,
                ReplyToSpam,
                ReplyFromSpam,
                ViewSpamReplies,
            };
        }
        
        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        PostTopics,
                        PostReplies,
                        EditOwnTopics,
                        EditAnyTopic,
                        EditOwnReplies,
                        EditAnyReply,
                        DeleteOwnTopics,
                        RestoreOwnTopics,
                        PermanentDeleteOwnTopics,
                        DeleteAnyTopic,
                        RestoreAnyTopic,
                        PermanentDeleteAnyTopic,
                        DeleteOwnReplies,
                        RestoreOwnReplies,
                        PermanentDeleteOwnReplies,
                        DeleteAnyReply,
                        RestoreAnyReply,
                        PermanentDeleteAnyReply,
                        ReportTopics,
                        ReportReplies,
                        ViewHiddenTopics,
                        ViewPrivateTopics,
                        ViewHiddenReplies,
                        ViewSpamTopics,
                        ViewSpamReplies,
                        ViewDeletedTopics,
                        ViewDeletedReplies,
                        PinTopics,
                        UnpinTopics,
                        LockTopics,
                        UnlockTopics,
                        HideTopics,
                        ShowTopics,
                        HideReplies,
                        ShowReplies,
                        TopicToSpam,
                        TopicFromSpam,
                        ReplyToSpam,
                        ReplyFromSpam
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostTopics,
                        PostReplies,
                        EditOwnTopics,
                        EditOwnReplies,
                        DeleteOwnTopics,
                        DeleteOwnReplies,
                        ReportTopics,
                        ReportReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostTopics,
                        PostReplies,
                        EditOwnTopics,
                        EditOwnReplies,
                        DeleteOwnTopics,
                        RestoreOwnTopics,
                        PermanentDeleteOwnTopics,
                        DeleteOwnReplies,
                        RestoreOwnReplies,
                        PermanentDeleteOwnReplies,
                        ReportTopics,
                        ReportReplies,
                        ViewHiddenTopics,
                        ViewPrivateTopics,
                        ViewHiddenReplies,
                        ViewSpamTopics,
                        ViewSpamReplies,
                        ViewDeletedTopics,
                        ViewDeletedReplies,
                        PinTopics,
                        UnpinTopics,
                        LockTopics,
                        UnlockTopics,
                        HideTopics,
                        ShowTopics,
                        HideReplies,
                        ShowReplies,
                        TopicToSpam,
                        TopicFromSpam,
                        ReplyToSpam,
                        ReplyFromSpam
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportTopics,
                        ReportReplies
                    }
                }
            };
        }

    }

}
