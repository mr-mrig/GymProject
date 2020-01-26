
-- The following operations must be performed via SQL scripting

--TrainingHashtag -> Body = UNIQUE

-- The following, for all the Entities attched to a PersonalNoteValue, GenericHashtagValue. Such as:
---- TrainingPlanNote -> Body NOT NULL
---- WorkUnitTemplateNote -> Body NOT NULL
---- TrainingPlanMessage -> Body NOT NULL
---- TrainingHashtag -> Body NOT NULL
---- WorkingSetNote -> Body NOT NULL
---- TrainingPlanNote -> Body NOT NULL
---- ...
---- The alternative is to remove the VO and simply use a plain string property

-- TrainingSchedule -> Index = (Start, TrainingPlanId), StartDate NOT NULL

-- UserTrainingPhase -> UQ: (UserId, Start)

-- UserTrainingProficiency -> UQ: (UserId, Start)

-- TrainingPhase -> Data Seeding

-- TrainingProficiency -> Data Seeding

-- IntensityTechnique -> Data Seeding

-- WorkingSetTemplate -> Rename column "Effort_EffortTypeId", "Repetitions_WorkTypeId"

--All the Many-to-Many relations -> Redundant indexes, should be removed

-- Training Week -> Redundant index, should be removed

--
--	How to Create the SQL functions?? RmToIntPerc etc....
--

