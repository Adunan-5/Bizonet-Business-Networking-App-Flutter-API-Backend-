SET IDENTITY_INSERT FollowupComments ON;

INSERT INTO FollowupComments (FollowupCommentsId, FollowupStatusId, FollowupCommentsName, So, Active) VALUES
(1, 1, 'In  Negotiation', 1, 1),
(2, 1, 'Line Busy', 2, 1),
(3, 1, 'Out Of Station', 3, 1),
(4, 1, 'Not Reachable', 4, 1),
(5, 1, 'Contact Later', 5, 1),
(6, 2, 'Poor Response', 1, 1),
(7, 2, 'Always Not Available', 2, 1),
(8, 2, 'No Budget', 3, 1),
(9, 2, 'Already Got Vendor', 4, 1),
(10, 3, 'Not Required at the Moment', 1, 1),
(11, 3, 'No Budget', 2, 1),
(12, 3, 'Other Reason', 3, 1),
(13, 4, 'Quality', 1, 1),
(14, 4, 'Suitable Price', 2, 1),
(15, 4, 'Good Support', 3, 1),
(16, 4, 'All of the Above', 4, 1),
(17, 5, 'Price is High', 1, 1),
(18, 5, 'Delayed Reach', 2, 1),
(19, 5, 'Already got Vendor', 3, 1),
(20, 5, 'Business Closed', 4, 1);

SET IDENTITY_INSERT FollowupComments OFF;
