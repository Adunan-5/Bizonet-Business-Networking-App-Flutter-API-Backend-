SET IDENTITY_INSERT FollowupStatuses ON;

INSERT INTO FollowupStatuses (FollowupStatusId, FollowupStatusName, So, Active) VALUES
(1, 'In Followup', 1, 1),
(2, 'Not Interested', 2, 1),
(3, 'Postponed', 3, 1),
(4, 'Won', 4, 1),
(5, 'Lost', 5, 1);

SET IDENTITY_INSERT FollowupStatuses OFF;
