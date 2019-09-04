//using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using GymProject.Infrastructure.Persistence.EFContext.Model;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace GymProject.Infrastructure.Persistence.EFContext
//{
//    public partial class GymContext : DbContext
//    {

//        public const string DefaultSchema = "GymApp";


//        public GymContext()
//        {
//        }

//        public GymContext(DbContextOptions<GymContext> options)
//            : base(options)
//        {
            
//        }


//        public virtual DbSet<AccountStatusType> AccountStatusType { get; set; }
//        public virtual DbSet<ActivityDay> ActivityDay { get; set; }
//        public virtual DbSet<BiaDevice> BiaDevice { get; set; }
//        public virtual DbSet<BiaDeviceBrand> BiaDeviceBrand { get; set; }
//        public virtual DbSet<BiaDeviceType> BiaDeviceType { get; set; }
//        public virtual DbSet<BiaEntry> BiaEntry { get; set; }
//        public virtual DbSet<Circumference> Circumference { get; set; }
//        public virtual DbSet<Comment> Comment { get; set; }
//        public virtual DbSet<DietDay> DietDay { get; set; }
//        public virtual DbSet<DietDayExample> DietDayExample { get; set; }
//        public virtual DbSet<DietDayMealExample> DietDayMealExample { get; set; }
//        public virtual DbSet<DietDayType> DietDayType { get; set; }
//        public virtual DbSet<DietHasHashtag> DietHasHashtag { get; set; }
//        public virtual DbSet<DietHashtag> DietHashtag { get; set; }
//        public virtual DbSet<DietPlan> DietPlan { get; set; }
//        public virtual DbSet<DietPlanDay> DietPlanDay { get; set; }
//        public virtual DbSet<DietPlanDayExample> DietPlanDayExample { get; set; }
//        public virtual DbSet<DietPlanUnit> DietPlanUnit { get; set; }
//        public virtual DbSet<EffortType> EffortType { get; set; }
//        public virtual DbSet<EntryStatusType> EntryStatusType { get; set; }
//        public virtual DbSet<Excercise> Excercise { get; set; }
//        public virtual DbSet<ExcercisePersonalLibrary> ExcercisePersonalLibrary { get; set; }
//        public virtual DbSet<ExcerciseRelation> ExcerciseRelation { get; set; }
//        public virtual DbSet<ExcerciseSecondaryTarget> ExcerciseSecondaryTarget { get; set; }
//        public virtual DbSet<ExerciseDifficulty> ExerciseDifficulty { get; set; }
//        public virtual DbSet<ExerciseFocus> ExerciseFocus { get; set; }
//        public virtual DbSet<FitnessDayEntry> FitnessDayEntry { get; set; }
//        public virtual DbSet<Food> Food { get; set; }
//        public virtual DbSet<GenderType> GenderType { get; set; }
//        public virtual DbSet<Hashtag> Hashtag { get; set; }
//        public virtual DbSet<Image> Image { get; set; }
//        public virtual DbSet<IntensityTechnique> IntensityTechnique { get; set; }
//        public virtual DbSet<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate { get; set; }
//        public virtual DbSet<MealExample> MealExample { get; set; }
//        public virtual DbSet<MealExampleHasFood> MealExampleHasFood { get; set; }
//        public virtual DbSet<MealType> MealType { get; set; }
//        public virtual DbSet<MeasuresEntry> MeasuresEntry { get; set; }
//        public virtual DbSet<Mus> Mus { get; set; }
//        public virtual DbSet<Muscle> Muscle { get; set; }
//        public virtual DbSet<MuscleWorkType> MuscleWorkType { get; set; }
//        public virtual DbSet<PerformanceFocus> PerformanceFocus { get; set; }
//        public virtual DbSet<PerformanceType> PerformanceType { get; set; }
//        public virtual DbSet<PersonalRecord> PersonalRecord { get; set; }
//        public virtual DbSet<Phase> Phase { get; set; }
//        public virtual DbSet<Plicometry> Plicometry { get; set; }
//        public virtual DbSet<Post> Post { get; set; }
//        public virtual DbSet<PostHasHashtag> PostHasHashtag { get; set; }
//        public virtual DbSet<RelationStatus> RelationStatus { get; set; }
//        public virtual DbSet<TraineeHasHashtag> TraineeHasHashtag { get; set; }
//        public virtual DbSet<TraineeHashtag> TraineeHashtag { get; set; }
//        public virtual DbSet<Trainer> Trainer { get; set; }
//        public virtual DbSet<TrainingCollaboration> TrainingCollaboration { get; set; }
//        public virtual DbSet<TrainingEquipment> TrainingEquipment { get; set; }
//        public virtual DbSet<TrainingHashtag> TrainingHashtag { get; set; }
//        public virtual DbSet<TrainingMuscleFocus> TrainingMuscleFocus { get; set; }
//        public virtual DbSet<TrainingPlan> TrainingPlan { get; set; }
//        public virtual DbSet<TrainingPlanHasHashtag> TrainingPlanHasHashtag { get; set; }
//        public virtual DbSet<TrainingPlanHasPhase> TrainingPlanHasPhase { get; set; }
//        public virtual DbSet<TrainingPlanMessage> TrainingPlanMessage { get; set; }
//        public virtual DbSet<TrainingPlanNote> TrainingPlanNote { get; set; }
//        public virtual DbSet<TrainingPlanRelation> TrainingPlanRelation { get; set; }
//        public virtual DbSet<TrainingPlanRelationType> TrainingPlanRelationType { get; set; }
//        public virtual DbSet<TrainingPlanTargetProficiency> TrainingPlanTargetProficiency { get; set; }
//        public virtual DbSet<TrainingProficiency> TrainingProficiency { get; set; }
//        public virtual DbSet<TrainingSchedule> TrainingSchedule { get; set; }
//        public virtual DbSet<TrainingScheduleFeedback> TrainingScheduleFeedback { get; set; }
//        public virtual DbSet<TrainingWeek> TrainingWeek { get; set; }
//        public virtual DbSet<TrainingWeekType> TrainingWeekType { get; set; }
//        public virtual DbSet<User> User { get; set; }
//        public virtual DbSet<UserDetail> UserDetail { get; set; }
//        public virtual DbSet<UserHasProficiency> UserHasProficiency { get; set; }
//        public virtual DbSet<UserLike> UserLike { get; set; }
//        public virtual DbSet<UserPhase> UserPhase { get; set; }
//        public virtual DbSet<UserPhaseNote> UserPhaseNote { get; set; }
//        public virtual DbSet<UserRelation> UserRelation { get; set; }
//        public virtual DbSet<Weight> Weight { get; set; }
//        public virtual DbSet<WellnessDay> WellnessDay { get; set; }
//        public virtual DbSet<WellnessDayHasMus> WellnessDayHasMus { get; set; }
//        public virtual DbSet<WorkUnit> WorkUnit { get; set; }
//        public virtual DbSet<WorkUnitTemplate> WorkUnitTemplate { get; set; }
//        public virtual DbSet<WorkUnitTemplateNote> WorkUnitTemplateNote { get; set; }
//        public virtual DbSet<WorkingSet> WorkingSet { get; set; }
//        public virtual DbSet<WorkingSetNote> WorkingSetNote { get; set; }
//        public virtual DbSet<WorkingSetTemplate> WorkingSetTemplate { get; set; }
//        public virtual DbSet<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechnique { get; set; }
//        public virtual DbSet<WorkoutSession> WorkoutSession { get; set; }
//        public virtual DbSet<WorkoutTemplate> WorkoutTemplate { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlite("Data Source=C:\\Users\\rigom\\Documents\\rigm\\0. Gym App\\Databases\\MyGymApp.db;");
//            }
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

//            modelBuilder.Entity<AccountStatusType>(entity =>
//            {
//                entity.HasIndex(e => e.Description)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Description)
//                    .IsRequired()
//                    .HasColumnType("TEXT (25)");
//            });

//            modelBuilder.Entity<ActivityDay>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.ActivityDay)
//                    .HasForeignKey<ActivityDay>(d => d.Id);
//            });

//            modelBuilder.Entity<BiaDevice>(entity =>
//            {
//                entity.HasIndex(e => e.Model)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Model).IsRequired();

//                entity.HasOne(d => d.Brand)
//                    .WithMany(p => p.BiaDevice)
//                    .HasForeignKey(d => d.BrandId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.DeviceType)
//                    .WithMany(p => p.BiaDevice)
//                    .HasForeignKey(d => d.DeviceTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<BiaDeviceBrand>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<BiaDeviceType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<BiaEntry>(entity =>
//            {
//                entity.HasIndex(e => e.OwnerId)
//                    .HasName("IDX_BiaEntry_OwnerId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.BiaDevice)
//                    .WithMany(p => p.BiaEntry)
//                    .HasForeignKey(d => d.BiaDeviceId);

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.BiaEntry)
//                    .HasForeignKey<BiaEntry>(d => d.Id);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.BiaEntry)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<Circumference>(entity =>
//            {
//                entity.HasIndex(e => e.OwnerId)
//                    .HasName("IDX_Circumference_OwnerId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.Circumference)
//                    .HasForeignKey<Circumference>(d => d.Id);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.Circumference)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<Comment>(entity =>
//            {
//                entity.HasIndex(e => new { e.PostId, e.UserId, e.CreatedOn })
//                    .HasName("IDX_Comment_PostId_UserId_CreatedOn");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body)
//                    .IsRequired()
//                    .HasColumnType("TEXT (1000)");

//                entity.Property(e => e.CreatedOn).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.Property(e => e.LastUpdate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.Property(e => e.PostId).HasColumnType("BIGINT");

//                entity.Property(e => e.UserId).HasDefaultValueSql("1");

//                entity.HasOne(d => d.Post)
//                    .WithMany(p => p.Comment)
//                    .HasForeignKey(d => d.PostId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.Comment)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<DietDay>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.IsFreeMeal).HasDefaultValueSql("0");

//                entity.HasOne(d => d.DietDayType)
//                    .WithMany(p => p.DietDay)
//                    .HasForeignKey(d => d.DietDayTypeId);

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.DietDay)
//                    .HasForeignKey<DietDay>(d => d.Id);
//            });

//            modelBuilder.Entity<DietDayExample>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.DietDayType)
//                    .WithMany(p => p.DietDayExample)
//                    .HasForeignKey(d => d.DietDayTypeId);
//            });

//            modelBuilder.Entity<DietDayMealExample>(entity =>
//            {
//                entity.HasKey(e => new { e.DietDayExampleId, e.MealExampleId });

//                entity.HasOne(d => d.DietDayExample)
//                    .WithMany(p => p.DietDayMealExample)
//                    .HasForeignKey(d => d.DietDayExampleId);

//                entity.HasOne(d => d.MealExample)
//                    .WithMany(p => p.DietDayMealExample)
//                    .HasForeignKey(d => d.MealExampleId);

//                entity.HasOne(d => d.MealType)
//                    .WithMany(p => p.DietDayMealExample)
//                    .HasForeignKey(d => d.MealTypeId);
//            });

//            modelBuilder.Entity<DietDayType>(entity =>
//            {
//                entity.HasIndex(e => e.Id)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<DietHasHashtag>(entity =>
//            {
//                entity.HasKey(e => new { e.DietPlanId, e.DietHashtagId });

//                entity.HasOne(d => d.DietHashtag)
//                    .WithMany(p => p.DietHasHashtag)
//                    .HasForeignKey(d => d.DietHashtagId);

//                entity.HasOne(d => d.DietPlan)
//                    .WithMany(p => p.DietHasHashtag)
//                    .HasForeignKey(d => d.DietPlanId);
//            });

//            modelBuilder.Entity<DietHashtag>(entity =>
//            {
//                entity.HasIndex(e => e.Body)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.DietHashtag)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Moderator)
//                    .WithMany(p => p.DietHashtag)
//                    .HasForeignKey(d => d.ModeratorId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<DietPlan>(entity =>
//            {
//                entity.HasIndex(e => new { e.CreatedOn, e.OwnerId })
//                    .HasName("IDX_DietPlan_CreatedOn_OwnerId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.CreatedOn).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.Property(e => e.Name)
//                    .IsRequired()
//                    .HasDefaultValueSql("'DietPlan'");

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.DietPlan)
//                    .HasForeignKey<DietPlan>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.DietPlan)
//                    .HasForeignKey(d => d.OwnerId);
//            });

//            modelBuilder.Entity<DietPlanDay>(entity =>
//            {
//                entity.HasIndex(e => new { e.DietPlanUnitId, e.DietDayTypeId })
//                    .HasName("IDX_DietPlanDay_DietPlanUnitId_DietDayTypeId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.DietDayType)
//                    .WithMany(p => p.DietPlanDay)
//                    .HasForeignKey(d => d.DietDayTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.DietPlanUnit)
//                    .WithMany(p => p.DietPlanDay)
//                    .HasForeignKey(d => d.DietPlanUnitId);
//            });

//            modelBuilder.Entity<DietPlanDayExample>(entity =>
//            {
//                entity.HasKey(e => new { e.DietPlanId, e.DietDayExampleId });

//                entity.HasOne(d => d.DietDayExample)
//                    .WithMany(p => p.DietPlanDayExample)
//                    .HasForeignKey(d => d.DietDayExampleId);

//                entity.HasOne(d => d.DietPlan)
//                    .WithMany(p => p.DietPlanDayExample)
//                    .HasForeignKey(d => d.DietPlanId);
//            });

//            modelBuilder.Entity<DietPlanUnit>(entity =>
//            {
//                entity.HasIndex(e => new { e.DietPlanId, e.EndDate })
//                    .HasName("IDX_DietPlanUnit_EndDate_IsNull");

//                entity.HasIndex(e => new { e.DietPlanId, e.StartDate, e.EndDate })
//                    .HasName("IDX_DIetPlanUnit_DietPlanId_StartDate");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.DietPlan)
//                    .WithMany(p => p.DietPlanUnit)
//                    .HasForeignKey(d => d.DietPlanId);
//            });

//            modelBuilder.Entity<EffortType>(entity =>
//            {
//                entity.HasIndex(e => e.Abbreviation)
//                    .IsUnique();

//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Abbreviation).IsRequired();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<EntryStatusType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Description).IsRequired();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<Excercise>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.HasIndex(e => new { e.MuscleId, e.ExcerciseDifficultyId, e.TrainingEquipmentId })
//                    .HasName("IDX_Excercise_MuscleId_ExcerciseDifficultyId_TrainingEquipmentId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.Excercise)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.ExcerciseDifficulty)
//                    .WithMany(p => p.Excercise)
//                    .HasForeignKey(d => d.ExcerciseDifficultyId);

//                entity.HasOne(d => d.Muscle)
//                    .WithMany(p => p.Excercise)
//                    .HasForeignKey(d => d.MuscleId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.PerformanceType)
//                    .WithMany(p => p.Excercise)
//                    .HasForeignKey(d => d.PerformanceTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.TrainingEquipment)
//                    .WithMany(p => p.Excercise)
//                    .HasForeignKey(d => d.TrainingEquipmentId);
//            });

//            modelBuilder.Entity<ExcercisePersonalLibrary>(entity =>
//            {
//                entity.HasKey(e => new { e.UserId, e.ExcerciseId });

//                entity.HasOne(d => d.Excercise)
//                    .WithMany(p => p.ExcercisePersonalLibrary)
//                    .HasForeignKey(d => d.ExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.ExcercisePersonalLibrary)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<ExcerciseRelation>(entity =>
//            {
//                entity.HasKey(e => new { e.ParentExcerciseId, e.ChildExcerciseId });

//                entity.HasOne(d => d.ChildExcercise)
//                    .WithMany(p => p.ExcerciseRelationChildExcercise)
//                    .HasForeignKey(d => d.ChildExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.ParentExcercise)
//                    .WithMany(p => p.ExcerciseRelationParentExcercise)
//                    .HasForeignKey(d => d.ParentExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<ExcerciseSecondaryTarget>(entity =>
//            {
//                entity.HasKey(e => new { e.ExcerciseId, e.MuscleId });

//                entity.HasIndex(e => new { e.ExcerciseId, e.MuscleId, e.MuscleWorkTypeId })
//                    .HasName("IDX_ExcerciseSecondaryTarget_FullCovering");

//                entity.HasOne(d => d.Excercise)
//                    .WithMany(p => p.ExcerciseSecondaryTarget)
//                    .HasForeignKey(d => d.ExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Muscle)
//                    .WithMany(p => p.ExcerciseSecondaryTarget)
//                    .HasForeignKey(d => d.MuscleId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.MuscleWorkType)
//                    .WithMany(p => p.ExcerciseSecondaryTarget)
//                    .HasForeignKey(d => d.MuscleWorkTypeId);
//            });

//            modelBuilder.Entity<ExerciseDifficulty>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<ExerciseFocus>(entity =>
//            {
//                entity.HasKey(e => new { e.PerformanceFocusId, e.ExerciseId });

//                entity.HasIndex(e => new { e.ExerciseId, e.PerformanceFocusId })
//                    .HasName("IDX_ExcerciseFocus_FullCovering");

//                entity.HasOne(d => d.Exercise)
//                    .WithMany(p => p.ExerciseFocus)
//                    .HasForeignKey(d => d.ExerciseId);

//                entity.HasOne(d => d.PerformanceFocus)
//                    .WithMany(p => p.ExerciseFocus)
//                    .HasForeignKey(d => d.PerformanceFocusId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<FitnessDayEntry>(entity =>
//            {
//                entity.HasIndex(e => e.DayDate)
//                    .HasName("IDX_FitnessDayEntry_DayDate");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.DayDate).HasDefaultValueSql("strftime('%s', CURRENT_DATE)");

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.FitnessDayEntry)
//                    .HasForeignKey<FitnessDayEntry>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<Food>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.Food)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<GenderType>(entity =>
//            {
//                entity.HasIndex(e => e.Abbreviation)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Abbreviation).IsRequired();
//            });

//            modelBuilder.Entity<Hashtag>(entity =>
//            {
//                entity.HasIndex(e => e.Body)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body)
//                    .IsRequired()
//                    .HasColumnType("TEXT (100)");

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.Hashtag)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Moderator)
//                    .WithMany(p => p.Hashtag)
//                    .HasForeignKey(d => d.ModeratorId);
//            });

//            modelBuilder.Entity<Image>(entity =>
//            {
//                entity.HasIndex(e => new { e.PostId, e.IsProgressPicture })
//                    .HasName("IDX_Image_PostId_CreatedOn_IsProgressPicture");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Url).IsRequired();

//                entity.HasOne(d => d.Post)
//                    .WithMany(p => p.Image)
//                    .HasForeignKey(d => d.PostId);
//            });

//            modelBuilder.Entity<IntensityTechnique>(entity =>
//            {
//                entity.HasIndex(e => e.Abbreviation)
//                    .IsUnique();

//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Abbreviation).IsRequired();

//                entity.Property(e => e.Name).IsRequired();

//                entity.Property(e => e.Rpe).HasColumnName("RPE");

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.IntensityTechnique)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<LinkedWorkUnitTemplate>(entity =>
//            {
//                entity.HasKey(e => new { e.FirstWorkUnitId, e.SecondWorkUnitId });

//                entity.HasIndex(e => new { e.FirstWorkUnitId, e.SecondWorkUnitId, e.IntensityTechniqueId })
//                    .HasName("IDX_LinkedWorkUnitTemplate_FullCovering");

//                entity.HasOne(d => d.FirstWorkUnit)
//                    .WithMany(p => p.LinkedWorkUnitTemplateFirstWorkUnit)
//                    .HasForeignKey(d => d.FirstWorkUnitId);

//                entity.HasOne(d => d.IntensityTechnique)
//                    .WithMany(p => p.LinkedWorkUnitTemplate)
//                    .HasForeignKey(d => d.IntensityTechniqueId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.SecondWorkUnit)
//                    .WithMany(p => p.LinkedWorkUnitTemplateSecondWorkUnit)
//                    .HasForeignKey(d => d.SecondWorkUnitId);
//            });

//            modelBuilder.Entity<MealExample>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();
//            });

//            modelBuilder.Entity<MealExampleHasFood>(entity =>
//            {
//                entity.HasKey(e => new { e.MealExampleId, e.FoodId });

//                entity.HasOne(d => d.Food)
//                    .WithMany(p => p.MealExampleHasFood)
//                    .HasForeignKey(d => d.FoodId);

//                entity.HasOne(d => d.MealExample)
//                    .WithMany(p => p.MealExampleHasFood)
//                    .HasForeignKey(d => d.MealExampleId);
//            });

//            modelBuilder.Entity<MealType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<MeasuresEntry>(entity =>
//            {
//                entity.HasIndex(e => e.MeasureDate)
//                    .HasName("IDX_MeasureEntry_MeasureDate");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.MeasureDate).HasDefaultValueSql("strftime('%s', CURRENT_DATE)");

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.MeasuresEntry)
//                    .HasForeignKey<MeasuresEntry>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.MeasuresEntry)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<Mus>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.Mus)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<Muscle>(entity =>
//            {
//                entity.HasIndex(e => e.Abbreviation)
//                    .IsUnique();

//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Abbreviation).IsRequired();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<MuscleWorkType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<PerformanceFocus>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<PerformanceType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<PersonalRecord>(entity =>
//            {
//                entity.HasIndex(e => new { e.UserId, e.ExcerciseId, e.RecordDate })
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.RecordDate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.HasOne(d => d.Excercise)
//                    .WithMany(p => p.PersonalRecord)
//                    .HasForeignKey(d => d.ExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.PersonalRecord)
//                    .HasForeignKey(d => d.UserId);
//            });

//            modelBuilder.Entity<Phase>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.HasIndex(e => e.OwnerId)
//                    .HasName("IDX_Phase_OwnerId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.Phase)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.Phase)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<Plicometry>(entity =>
//            {
//                entity.HasIndex(e => e.OwnerId)
//                    .HasName("IDX_Plicometry_OwnerId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.Plicometry)
//                    .HasForeignKey<Plicometry>(d => d.Id);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.Plicometry)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<Post>(entity =>
//            {
//                entity.HasIndex(e => new { e.UserId, e.CreatedOn })
//                    .HasName("IDX_Post_UserId_CreatedOn");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Caption).HasColumnType("TEXT (1000)");

//                entity.Property(e => e.CreatedOn).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.Property(e => e.IsPublic).HasColumnType("BOOLEAN");

//                entity.Property(e => e.LastUpdate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.Post)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<PostHasHashtag>(entity =>
//            {
//                entity.HasKey(e => new { e.PostId, e.HashtagId });

//                entity.HasOne(d => d.Hashtag)
//                    .WithMany(p => p.PostHasHashtag)
//                    .HasForeignKey(d => d.HashtagId);

//                entity.HasOne(d => d.Post)
//                    .WithMany(p => p.PostHasHashtag)
//                    .HasForeignKey(d => d.PostId);
//            });

//            modelBuilder.Entity<RelationStatus>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<TraineeHasHashtag>(entity =>
//            {
//                entity.HasKey(e => new { e.HashtagId, e.TraineeId, e.TrainerId });

//                entity.HasOne(d => d.Hashtag)
//                    .WithMany(p => p.TraineeHasHashtag)
//                    .HasForeignKey(d => d.HashtagId);

//                entity.HasOne(d => d.Trainee)
//                    .WithMany(p => p.TraineeHasHashtag)
//                    .HasForeignKey(d => d.TraineeId);

//                entity.HasOne(d => d.Trainer)
//                    .WithMany(p => p.TraineeHasHashtag)
//                    .HasForeignKey(d => d.TrainerId);
//            });

//            modelBuilder.Entity<TraineeHashtag>(entity =>
//            {
//                entity.HasIndex(e => e.Body)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.TraineeHashtag)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Moderator)
//                    .WithMany(p => p.TraineeHashtag)
//                    .HasForeignKey(d => d.ModeratorId);
//            });

//            modelBuilder.Entity<Trainer>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.Trainer)
//                    .HasForeignKey<Trainer>(d => d.Id);
//            });

//            modelBuilder.Entity<TrainingCollaboration>(entity =>
//            {
//                entity.HasIndex(e => new { e.EndDate, e.TraineeId, e.TrainerId })
//                    .HasName("IDX_TrainingCollaboration_EndDate_TraineeId_TrainerId");

//                entity.HasIndex(e => new { e.StartDate, e.TrainerId, e.TraineeId })
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.StartDate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.HasOne(d => d.Trainee)
//                    .WithMany(p => p.TrainingCollaboration)
//                    .HasForeignKey(d => d.TraineeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Trainer)
//                    .WithMany(p => p.TrainingCollaboration)
//                    .HasForeignKey(d => d.TrainerId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<TrainingEquipment>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<TrainingHashtag>(entity =>
//            {
//                entity.HasIndex(e => e.Body)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();

//                entity.HasOne(d => d.EntryStatusType)
//                    .WithMany(p => p.TrainingHashtag)
//                    .HasForeignKey(d => d.EntryStatusTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Moderator)
//                    .WithMany(p => p.TrainingHashtag)
//                    .HasForeignKey(d => d.ModeratorId);
//            });

//            modelBuilder.Entity<TrainingMuscleFocus>(entity =>
//            {
//                entity.HasKey(e => new { e.TrainingPlanId, e.MuscleId });

//                entity.HasIndex(e => new { e.TrainingPlanId, e.MuscleId })
//                    .HasName("IDX_TrainingMuscleFocus_FullCovering");

//                entity.HasOne(d => d.Muscle)
//                    .WithMany(p => p.TrainingMuscleFocus)
//                    .HasForeignKey(d => d.MuscleId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.TrainingPlan)
//                    .WithMany(p => p.TrainingMuscleFocus)
//                    .HasForeignKey(d => d.TrainingPlanId);
//            });

//            modelBuilder.Entity<TrainingPlan>(entity =>
//            {
//                entity.HasIndex(e => new { e.Name, e.OwnerId })
//                    .IsUnique();

//                entity.HasIndex(e => new { e.OwnerId, e.TrainingPlanNoteId })
//                    .HasName("IDX_TrainingPlan_OwnerId_TrainingPlanNoteId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.CreatedOn).HasDefaultValueSql("strftime('%s', CURRENT_DATE)");

//                entity.Property(e => e.Name).IsRequired();

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.TrainingPlan)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.TrainingPlanNote)
//                    .WithMany(p => p.TrainingPlan)
//                    .HasForeignKey(d => d.TrainingPlanNoteId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<TrainingPlanHasHashtag>(entity =>
//            {
//                entity.HasKey(e => new { e.TrainingPlanId, e.TrainingHashtagId });

//                entity.HasOne(d => d.TrainingHashtag)
//                    .WithMany(p => p.TrainingPlanHasHashtag)
//                    .HasForeignKey(d => d.TrainingHashtagId);

//                entity.HasOne(d => d.TrainingPlan)
//                    .WithMany(p => p.TrainingPlanHasHashtag)
//                    .HasForeignKey(d => d.TrainingPlanId);
//            });

//            modelBuilder.Entity<TrainingPlanHasPhase>(entity =>
//            {
//                entity.HasKey(e => new { e.PlanId, e.PhaseId });

//                entity.HasIndex(e => new { e.PlanId, e.PhaseId })
//                    .HasName("IDX_TrainingPlanHasPhase_FullCovering");

//                entity.HasOne(d => d.Phase)
//                    .WithMany(p => p.TrainingPlanHasPhase)
//                    .HasForeignKey(d => d.PhaseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Plan)
//                    .WithMany(p => p.TrainingPlanHasPhase)
//                    .HasForeignKey(d => d.PlanId);
//            });

//            modelBuilder.Entity<TrainingPlanMessage>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();
//            });

//            modelBuilder.Entity<TrainingPlanNote>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();
//            });

//            modelBuilder.Entity<TrainingPlanRelation>(entity =>
//            {
//                entity.HasKey(e => new { e.ParentPlanId, e.ChildPlanId });

//                entity.HasIndex(e => new { e.ParentPlanId, e.ChildPlanId, e.TrainingPlanMessageId })
//                    .HasName("IDX_TrainingPlanRelation_ParentPlanId_ChildPlanId_TrainingPlanMessageId");

//                entity.HasOne(d => d.ChildPlan)
//                    .WithMany(p => p.TrainingPlanRelationChildPlan)
//                    .HasForeignKey(d => d.ChildPlanId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.ParentPlan)
//                    .WithMany(p => p.TrainingPlanRelationParentPlan)
//                    .HasForeignKey(d => d.ParentPlanId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.RelationType)
//                    .WithMany(p => p.TrainingPlanRelation)
//                    .HasForeignKey(d => d.RelationTypeId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.TrainingPlanMessage)
//                    .WithMany(p => p.TrainingPlanRelation)
//                    .HasForeignKey(d => d.TrainingPlanMessageId);
//            });

//            modelBuilder.Entity<TrainingPlanRelationType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Description).IsRequired();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<TrainingPlanTargetProficiency>(entity =>
//            {
//                entity.HasKey(e => new { e.TrainingPlanId, e.TrainingProficiencyId });

//                entity.HasIndex(e => new { e.TrainingPlanId, e.TrainingProficiencyId })
//                    .HasName("IDX_TrainingPlanTarget_FullCovering");

//                entity.HasOne(d => d.TrainingPlan)
//                    .WithMany(p => p.TrainingPlanTargetProficiency)
//                    .HasForeignKey(d => d.TrainingPlanId);

//                entity.HasOne(d => d.TrainingProficiency)
//                    .WithMany(p => p.TrainingPlanTargetProficiency)
//                    .HasForeignKey(d => d.TrainingProficiencyId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<TrainingProficiency>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<TrainingSchedule>(entity =>
//            {
//                entity.HasIndex(e => new { e.TrainingPlanId, e.PhaseId, e.TrainingProficiencyId })
//                    .HasName("IDX_TrainingSchedule_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId");

//                entity.HasIndex(e => new { e.StartDate, e.TrainingPlanId, e.PhaseId, e.TrainingProficiencyId })
//                    .HasName("IDX_TrainingSchedule_StartDate_TrainingPlanId_CurrentWeekId_PhaseId_TrainingProficiencyId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.TrainingSchedule)
//                    .HasForeignKey<TrainingSchedule>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Phase)
//                    .WithMany(p => p.TrainingSchedule)
//                    .HasForeignKey(d => d.PhaseId);

//                entity.HasOne(d => d.TrainingPlan)
//                    .WithMany(p => p.TrainingSchedule)
//                    .HasForeignKey(d => d.TrainingPlanId)
//                    .OnDelete(DeleteBehavior.SetNull);

//                entity.HasOne(d => d.TrainingProficiency)
//                    .WithMany(p => p.TrainingSchedule)
//                    .HasForeignKey(d => d.TrainingProficiencyId);
//            });

//            modelBuilder.Entity<TrainingScheduleFeedback>(entity =>
//            {
//                entity.HasIndex(e => new { e.TrainingScheduleId, e.UserId })
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.LastUpdate).HasDefaultValueSql("STRFTIME('%s', 'now')");

//                entity.HasOne(d => d.TrainingSchedule)
//                    .WithMany(p => p.TrainingScheduleFeedback)
//                    .HasForeignKey(d => d.TrainingScheduleId);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.TrainingScheduleFeedback)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<TrainingWeek>(entity =>
//            {
//                entity.HasIndex(e => new { e.ProgressiveNumber, e.TrainingPlanId })
//                    .IsUnique();

//                entity.HasIndex(e => new { e.TrainingPlanId, e.ProgressiveNumber })
//                    .HasName("IDX_TrainingWeekTemplate_TrainingPlanId_ProgressiveNumber");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.TrainingPlan)
//                    .WithMany(p => p.TrainingWeek)
//                    .HasForeignKey(d => d.TrainingPlanId);

//                entity.HasOne(d => d.TrainingWeekType)
//                    .WithMany(p => p.TrainingWeek)
//                    .HasForeignKey(d => d.TrainingWeekTypeId);
//            });

//            modelBuilder.Entity<TrainingWeekType>(entity =>
//            {
//                entity.HasIndex(e => e.Name)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Name).IsRequired();
//            });

//            modelBuilder.Entity<User>(entity =>
//            {
//                entity.HasIndex(e => e.Email)
//                    .IsUnique();

//                entity.HasIndex(e => e.Salt)
//                    .IsUnique();

//                entity.HasIndex(e => e.Username)
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Email).IsRequired();

//                entity.Property(e => e.Password)
//                    .IsRequired()
//                    .HasColumnType("TEXT (128)");

//                entity.Property(e => e.Salt)
//                    .IsRequired()
//                    .HasColumnType("TEXT (128)");

//                entity.Property(e => e.SubscriptionDate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.Property(e => e.Username).IsRequired();

//                entity.HasOne(d => d.AccountStatusType)
//                    .WithMany(p => p.User)
//                    .HasForeignKey(d => d.AccountStatusTypeId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<UserDetail>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.GenderType)
//                    .WithMany(p => p.UserDetail)
//                    .HasForeignKey(d => d.GenderTypeId);

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.UserDetail)
//                    .HasForeignKey<UserDetail>(d => d.Id);
//            });

//            modelBuilder.Entity<UserHasProficiency>(entity =>
//            {
//                entity.HasKey(e => new { e.UserId, e.ProficiencyId, e.OwnerId });

//                entity.Property(e => e.StartDate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.UserHasProficiencyOwner)
//                    .HasForeignKey(d => d.OwnerId);

//                entity.HasOne(d => d.Proficiency)
//                    .WithMany(p => p.UserHasProficiencyProficiency)
//                    .HasForeignKey(d => d.ProficiencyId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.UserHasProficiencyUser)
//                    .HasForeignKey(d => d.UserId);
//            });

//            modelBuilder.Entity<UserLike>(entity =>
//            {
//                entity.HasKey(e => new { e.PostId, e.UserId });

//                entity.HasIndex(e => new { e.PostId, e.UserId })
//                    .HasName("IDX_UserLike_PostId_UserId");

//                entity.Property(e => e.CreatedOn).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.HasOne(d => d.Post)
//                    .WithMany(p => p.UserLike)
//                    .HasForeignKey(d => d.PostId);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.UserLike)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.SetNull);
//            });

//            modelBuilder.Entity<UserPhase>(entity =>
//            {
//                entity.HasIndex(e => new { e.OwnerId, e.PhaseId, e.StartDate, e.UserPhaseNoteId })
//                    .HasName("IDX_UserPhase_OwnerId_PhaseId_StartDate_UserPhaseNoteId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.CreatedOn).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.Property(e => e.StartDate).HasDefaultValueSql("strftime('%s', CURRENT_DATE)");

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.UserPhase)
//                    .HasForeignKey<UserPhase>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Owner)
//                    .WithMany(p => p.UserPhase)
//                    .HasForeignKey(d => d.OwnerId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.Phase)
//                    .WithMany(p => p.UserPhase)
//                    .HasForeignKey(d => d.PhaseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.UserPhaseNote)
//                    .WithMany(p => p.UserPhase)
//                    .HasForeignKey(d => d.UserPhaseNoteId);
//            });

//            modelBuilder.Entity<UserPhaseNote>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();
//            });

//            modelBuilder.Entity<UserRelation>(entity =>
//            {
//                entity.HasKey(e => new { e.SourceUserId, e.TargetUserId });

//                entity.HasIndex(e => new { e.SourceUserId, e.TargetUserId, e.RelationStatusId })
//                    .HasName("IDX_UserRelation_SourceUserId_TargetUserId_RelationStatusId");

//                entity.Property(e => e.StartDate).HasDefaultValueSql("strftime('%s', 'now')");

//                entity.HasOne(d => d.RelationStatus)
//                    .WithMany(p => p.UserRelation)
//                    .HasForeignKey(d => d.RelationStatusId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.SourceUser)
//                    .WithMany(p => p.UserRelationSourceUser)
//                    .HasForeignKey(d => d.SourceUserId);

//                entity.HasOne(d => d.TargetUser)
//                    .WithMany(p => p.UserRelationTargetUser)
//                    .HasForeignKey(d => d.TargetUserId);
//            });

//            modelBuilder.Entity<Weight>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.Weight)
//                    .HasForeignKey<Weight>(d => d.Id);

//                entity.HasOne(d => d.Id1)
//                    .WithOne(p => p.Weight)
//                    .HasForeignKey<Weight>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);
//            });

//            modelBuilder.Entity<WellnessDay>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.WellnessDay)
//                    .HasForeignKey<WellnessDay>(d => d.Id);
//            });

//            modelBuilder.Entity<WellnessDayHasMus>(entity =>
//            {
//                entity.HasKey(e => new { e.WellnessDayId, e.MusId });

//                entity.HasIndex(e => new { e.WellnessDayId, e.MusId })
//                    .HasName("IDX_WellnessDayHasMus_WellnessDayId_MusId");

//                entity.HasOne(d => d.Mus)
//                    .WithMany(p => p.WellnessDayHasMus)
//                    .HasForeignKey(d => d.MusId);

//                entity.HasOne(d => d.WellnessDay)
//                    .WithMany(p => p.WellnessDayHasMus)
//                    .HasForeignKey(d => d.WellnessDayId);
//            });

//            modelBuilder.Entity<WorkUnit>(entity =>
//            {
//                entity.HasIndex(e => new { e.ProgressiveNumber, e.WorkoutSessionId })
//                    .IsUnique();

//                entity.HasIndex(e => new { e.WorkoutSessionId, e.ExcerciseId, e.ProgressiveNumber })
//                    .HasName("IDX_WorkUnit_WorkoutSessionId_ExcerciseId_ProgressiveNumber");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.Excercise)
//                    .WithMany(p => p.WorkUnit)
//                    .HasForeignKey(d => d.ExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.WorkoutSession)
//                    .WithMany(p => p.WorkUnit)
//                    .HasForeignKey(d => d.WorkoutSessionId);
//            });

//            modelBuilder.Entity<WorkUnitTemplate>(entity =>
//            {
//                entity.HasIndex(e => new { e.ProgressiveNumber, e.WorkoutTemplateId })
//                    .IsUnique();

//                entity.HasIndex(e => new { e.WorkoutTemplateId, e.ExcerciseId, e.ProgressiveNumber })
//                    .HasName("IDX_WorkUnitTemplate_WorkoutTemplateId_ExcerciseId_ProgressiveNumber");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.Excercise)
//                    .WithMany(p => p.WorkUnitTemplate)
//                    .HasForeignKey(d => d.ExcerciseId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.WorkUnitTemplateNote)
//                    .WithMany(p => p.WorkUnitTemplate)
//                    .HasForeignKey(d => d.WorkUnitTemplateNoteId)
//                    .OnDelete(DeleteBehavior.SetNull);

//                entity.HasOne(d => d.WorkoutTemplate)
//                    .WithMany(p => p.WorkUnitTemplate)
//                    .HasForeignKey(d => d.WorkoutTemplateId);
//            });

//            modelBuilder.Entity<WorkUnitTemplateNote>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();
//            });

//            modelBuilder.Entity<WorkingSet>(entity =>
//            {
//                entity.HasIndex(e => new { e.ProgressiveNumber, e.WorkUnitId })
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.WorkUnit)
//                    .WithMany(p => p.WorkingSet)
//                    .HasForeignKey(d => d.WorkUnitId);
//            });

//            modelBuilder.Entity<WorkingSetNote>(entity =>
//            {
//                entity.HasIndex(e => e.WorkingSetId)
//                    .HasName("IDX_WorkingSetNote_WorkingSetId");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.Body).IsRequired();

//                entity.HasOne(d => d.WorkingSet)
//                    .WithMany(p => p.WorkingSetNote)
//                    .HasForeignKey(d => d.WorkingSetId);
//            });

//            modelBuilder.Entity<WorkingSetTemplate>(entity =>
//            {
//                entity.HasIndex(e => new { e.WorkUnitTemplateId, e.ProgressiveNumber })
//                    .IsUnique();

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.EffortType)
//                    .WithMany(p => p.WorkingSetTemplate)
//                    .HasForeignKey(d => d.EffortTypeId);

//                entity.HasOne(d => d.WorkUnitTemplate)
//                    .WithMany(p => p.WorkingSetTemplate)
//                    .HasForeignKey(d => d.WorkUnitTemplateId);
//            });

//            modelBuilder.Entity<WorkingSetTemplateIntensityTechnique>(entity =>
//            {
//                entity.HasKey(e => new { e.SetTemplateId, e.IntensityTechniqueId });

//                entity.HasIndex(e => new { e.SetTemplateId, e.LinkedSetTemplateId, e.IntensityTechniqueId })
//                    .HasName("IDX_SetTemplateIntensityTechnique_FullCovering");

//                entity.HasOne(d => d.IntensityTechnique)
//                    .WithMany(p => p.WorkingSetTemplateIntensityTechnique)
//                    .HasForeignKey(d => d.IntensityTechniqueId)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.LinkedSetTemplate)
//                    .WithMany(p => p.WorkingSetTemplateIntensityTechniqueLinkedSetTemplate)
//                    .HasForeignKey(d => d.LinkedSetTemplateId)
//                    .OnDelete(DeleteBehavior.Cascade);

//                entity.HasOne(d => d.SetTemplate)
//                    .WithMany(p => p.WorkingSetTemplateIntensityTechniqueSetTemplate)
//                    .HasForeignKey(d => d.SetTemplateId);
//            });

//            modelBuilder.Entity<WorkoutSession>(entity =>
//            {
//                entity.HasIndex(e => new { e.WorkoutTemplateId, e.StartTime })
//                    .HasName("IDX_WorkoutSession_WorkoutTemplateId_StartTime");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.Property(e => e.StartTime).HasDefaultValueSql("STRFTIME('%s', 'now')");

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.WorkoutSession)
//                    .HasForeignKey<WorkoutSession>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull);

//                entity.HasOne(d => d.WorkoutTemplate)
//                    .WithMany(p => p.WorkoutSession)
//                    .HasForeignKey(d => d.WorkoutTemplateId);
//            });

//            modelBuilder.Entity<WorkoutTemplate>(entity =>
//            {
//                entity.HasIndex(e => new { e.ProgressiveNumber, e.TrainingWeekId })
//                    .IsUnique();

//                entity.HasIndex(e => new { e.TrainingWeekId, e.ProgressiveNumber })
//                    .HasName("IDX_WorkoutTemplate_TrainingWeekId_ProressiveNumber");

//                entity.Property(e => e.Id).ValueGeneratedOnAdd();

//                entity.HasOne(d => d.TrainingWeek)
//                    .WithMany(p => p.WorkoutTemplate)
//                    .HasForeignKey(d => d.TrainingWeekId);
//            });
//        }
//    }
//}
