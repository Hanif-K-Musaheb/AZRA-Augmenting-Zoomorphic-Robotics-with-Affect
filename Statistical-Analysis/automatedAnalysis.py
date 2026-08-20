from SpeechStatAnalysis import speech_statistical_anylysis


# columns_to_test = [
#     'Vocabulary Variety - total_words'
#     # 'Speech Pace - duration_seconds',
#     # 'Sentence Complexity - avg_sentence_length'
# ]


columns_to_test = [
    'Vocabulary Variety - unique_words', 'Vocabulary Variety - total_words', 'Vocabulary Variety - ratio',
    'Sentence Complexity - connectors', 'Speech Pace - units_per_minute', 'Speech Pace - duration_seconds',
    'Pause Pattern - voiced_seconds', 'Pause Pattern - pause_seconds',
    'Pause Pattern - pause_count', 'Pause Pattern - mean_pause_seconds', 'Pause Pattern - max_pause_seconds',
    'Pause Pattern - pause_ratio', 'Pause Pattern - pause_analysis', 'Pause Pattern - neutral_score_used', 'Repetition Pattern - repeated_instances',
    'Repetition Pattern - ratio', 'Repetition Pattern - repeated_words', 'Repetition Pattern - total_words',
    'Emotional Tone - positive_words', 'Emotional Tone - negative_words', 'Emotional Tone - neutral_score_used',
    
]

stats_engine = speech_statistical_anylysis('Tall_labelled_feature.csv', columns_to_test)

stats_engine.print_significant_effect_sizes_table()
