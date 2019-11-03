
-- The following operations must be performed via SQL scripting

--TrainingHashtag -> Body = UNIQUE

--TrainingSchedule -> Index = (Start, TrainingPlanId), StartDate NOT NULL

--TrainingPlanMessage -> Body NOT NULL

--TrainingPlanNote -> Body NOT NULL

--WorkUnitTemplateNote -> Body NOT NULL

-- TrainingPhase -> Data Seeding

-- TrainingProficiency -> Data Seeding

-- IntensityTechnique -> Data Seeding

-- WorkingSetTemplate -> Rename column "Effort_EffortTypeId"

--All the Many-to-Many relations -> Redundant indexes, should be removed

-- Training Week -> Redundant index, should be removed

--
--	How to Create the SQL functions?? RmToIntPErc etc....
--

