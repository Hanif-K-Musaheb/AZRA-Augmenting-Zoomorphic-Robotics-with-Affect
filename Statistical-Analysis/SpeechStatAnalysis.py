import pandas as pd
from statsmodels.stats.anova import AnovaRM
from statsmodels.stats.multicomp import pairwise_tukeyhsd
import numpy as np
from cohens_d import cohens_d
import itertools



class speech_statistical_anylysis:
    def __init__(self, CSV_name,target_col):
        self.CSV_name = CSV_name
        self.target_col = target_col
        #self.df_balanced = self.clean_data()
        self.raw_df = pd.read_csv(self.CSV_name)
        
    def clean_data(self,single_col):
        """
        Clean the input dataframe by keeping only relevant columns and dropping empty rows.
        Clean data for one specific column at a time.
        
        Parameters:
        df (pd.DataFrame): The input dataframe to clean.
        single_col (str): The name of the column to keep.
        
        Returns:
        pd.DataFrame: The cleaned dataframe with only relevant columns and no empty rows.
        """
        # 1. Isolate the columns (use .copy() to avoid Pandas warnings)
        df_clean = self.raw_df[['ParticipantID', 'Feature', single_col]].copy()

        # 2. Force the target column to be numeric (True/False becomes 1/0)
        df_clean[single_col] = pd.to_numeric(df_clean[single_col], errors='coerce')

        # 3. Drop empty rows and any text rows that couldn't be converted
        df_clean = df_clean.dropna()

        # 4. Balance the dataset as usual
        feature_counts = df_clean.groupby('ParticipantID')['Feature'].nunique()
        complete_participants = feature_counts[feature_counts == 5].index
        df_balanced = df_clean[df_clean['ParticipantID'].isin(complete_participants)]

        return df_balanced
    

    def perform_anova(self, df, target_col):
        """
        Perform Repeated Measures ANOVA on the given dataframe and target column.
        
        Parameters:
        df (pd.DataFrame): The dataframe containing the data.
        target_col (str): The name of the target column for the ANOVA.
        
        Returns:
        exact_p_value (float): The exact p-value from the ANOVA results.
        """
        # 3. Perform the Repeated Measures ANOVA
        print("--- Repeated Measures ANOVA ---")
        aovrm = AnovaRM(
            data=df, 
            depvar=target_col, 
            subject='ParticipantID', 
            within=['Feature']
        )

        res = aovrm.fit()
        print(res)

        # Extract and print the exact p-value from the results table
        exact_p_value = res.anova_table['Pr > F']['Feature']
        print(f"Exact p-value: {exact_p_value}\n")
        return exact_p_value



    def tukey_hsd(self, df, target_col):
        """
        Perform Tukey's HSD test on the given dataframe and target column.
        
        Parameters:
        df (pd.DataFrame): The dataframe containing the data.
        target_col (str): The name of the target column for the test.
        
        Returns:
        None: Prints the results of Tukey's HSD test.
        """
        # Using the df_balanced dataframe from the previous step
        tukey = pairwise_tukeyhsd(endog=df[target_col], 
                                groups=df['Feature'], 
                                alpha=0.05)
        print(tukey)

    def interpret_cohens_d(self, d):
        """Interpret Cohen's d effect size."""
        abs_d = abs(d)
        if abs_d < 0.2:
            return "negligible"
        elif abs_d < 0.5:
            return "small"
        elif abs_d < 0.8:
            return "medium"
        else:
            return "large"

    def perform_cohens_d(self, column, feature1, feature2):
        """
        Perform Cohen's d effect size calculation between two features for a given column.
        
        Parameters:
        column (str): The name of the column for which to calculate the effect size.
        feature1 (str): The name of the first feature.
        feature2 (str): The name of the second feature.
        
        Returns:
        None: Prints the effect size and its interpretation.
        """
        data_tv = self.df_balanced[self.df_balanced['Feature'] == feature1].sort_values('ParticipantID')[column].values
        data_train = self.df_balanced[self.df_balanced['Feature'] == feature2].sort_values('ParticipantID')[column].values

        effect_size = cohens_d(data_tv, data_train, paired=True)#note: the paired argument is set to True because the data is from the same participants across different features

        interpretation = self.interpret_cohens_d(effect_size)
        print(f"{feature1:<10} | {feature2:<10} | {effect_size:<10.2f} | {interpretation}") 
    

    def run_analysis(self, alpha=0.05):
        """
        Run the complete statistical analysis including ANOVA, Tukey's HSD, and Cohen's d.
        """
        for col in self.target_col:
            print(f"\n{'='*50}")
            print(f"Running analysis for column: {col}")
            print(f"{'='*50}")
            
            # Clean the data JUST for this specific column
            self.df_balanced = self.clean_data(col)
            
            # Prevent the crash if a column has absolutely no valid data
            if self.df_balanced.empty:
                print(f"Not enough complete data to analyze {col}. Skipping.")
                continue

            # Run the ANOVA
            exact_p_value = self.perform_anova(self.df_balanced, col)
            
            # The rest of your loop remains exactly the same...
            if exact_p_value < alpha:
                self.tukey_hsd(self.df_balanced, col)
                self.perform_cohens_d(col, 'train', 'tv')

                # Use the hardcoded 'Feature' string since self.feature_col doesn't exist
                features = sorted(self.df_balanced['Feature'].unique())
                pairs = list(itertools.combinations(features, 2))
                
                for f1, f2 in pairs:
                    self.perform_cohens_d(col, f1, f2)

            else:
                print(f"*** No statistical significance found for {col}. Skipping post-hoc tests. ***")

    
