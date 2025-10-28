-- PostgreSQL compatible insert script for "SewaTypes"
INSERT INTO "SewaTypes" ("Id", "SewaName") VALUES
  (1, 'SHK (5:00 AM to 8:30 AM)'),
  (2, 'SHK (7:00 AM to 1:00 PM)'),
  (3, 'HRT (5:00 AM to 8:30 AM)'),
  (4, 'HRT (7:00 AM to 1:00 PM)'),
  (5, 'SEC - 24x7 (6:00 AM to 6:00 AM)'),
  (6, 'LGR (8:00 AM to 3:00 PM)'),
  (7, 'CTN (8:00 AM to 3:00 PM)'),
  (8, 'CTN - COOKING (8:00 AM to 3:00 PM)'),
  (9, 'WTR (7:00 AM to 1:00 PM)'),
  (10, 'PROJECTS (7:00 AM to 1:00 PM)'),
  (11, 'FOR BHATI - BICHHAI / KANAT REPAIR'),
  (12, 'PREVIOUS DAY SECURITY'),
  (13, 'SEWA @ OWN LAND CENTRES'),
  (14, 'SEC - EVE (6:00 PM TO 6:00 AM)');


SELECT setval(pg_get_serial_sequence('"SewaTypes"', 'Id'), (SELECT MAX("Id") FROM "SewaTypes"));
